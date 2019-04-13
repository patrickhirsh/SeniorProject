using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RideShareLevel
{
    public class Vehicle : LevelObject
    {
        public Route CurrentRoute;              // the route this vehicle is currently on
        public Connection CurrentConnection;    // the connection this vehicle is currently on

        public float Speed = 5f;                // the speed at which this vehicle will traverse it's current path
        public float RotationSpeed = 10f;                // the speed at which this vehicle will traverse it's current path
        public float Granularity = .005f;
        public float BaseSpeed = 5f;            // the speed this car will travel at its fastest

        public Gradient PickupGradient;
        public Gradient DropoffGradient;
        public GameObject RingPrefab;

        public LineRenderer VehiclePathLine;
        public BezierCurve VehiclePath;
        public float PathCompletionPercent;
        public bool PathIsComplete => PathCompletionPercent >= 1;
        public Vector3 NextPosition;
        private bool _canMove;

        public HashSet<Passenger> Passengers;
        public bool HasPassengers => Passengers != null && Passengers.Any();
        public bool PlayerControlled => Controller.GetType() == typeof(PlayerVehicleController);
        public bool NeutralControlled => Controller.GetType() == typeof(NeutralVehicleController);
        public VehicleController Controller;

        private Vector3 _startingPos;
        private Quaternion _startingRot;
        private GameObject _ring;
        //the speed at which the selection ring will pulse when it's being hovered on
        public float ringPulseSpeedDefault;
        public float ringPulseSpeedOnHover;
        public Color defaultRingPulseColor;
        public Color HoverRingPulseColor;

        private AudioSource AudioSource;

#if UNITY_EDITOR
        public void Bake()
        {
            UnityEditor.Undo.RecordObject(this, "Bake Vehicle Controller");
            Controller = GetComponentInParent<VehicleController>();
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            AudioSource = this.GetComponent<AudioSource>();
        }
#endif

        #region Unity Methods

        protected void Awake()
        {
            CurrentTask = null;
            Passengers = new HashSet<Passenger>();

            Broadcaster.AddListener(GameEvent.Reset, Reset);
            Broadcaster.AddListener(GameEvent.GameStateChanged, GameStateChanged);
            InputManager.Instance.HoverChanged.AddListener(ChangeRingColor);

            _ring = SpawnRing(defaultRingPulseColor, ringPulseSpeedDefault);
            _ring.transform.parent = transform;
            _ring.SetActive(false);
        }


        private void Update()
        {
            if (!HasTask) return;

            if (!CurrentTask.IsComplete())
            {
                if (_canMove && PathCompletionPercent < 1)
                {
                    Move();
                }
            }
            else if (CurrentTask.IsComplete())
            {
                CompleteTask();
            }
        }

        #endregion

        #region Task System

        public VehicleTask CurrentTask { get; private set; }
        public bool HasTask => CurrentTask != null;

        private Queue<VehicleTask> _tasks = new Queue<VehicleTask>();

        public void AddTask(VehicleTask task)
        {
            _tasks.Enqueue(task);
            RunNextTask();
        }

        public void SetTask(VehicleTask task, bool clearQueue = false)
        {
            CurrentTask = task;
            if (clearQueue)
            {
                _tasks.Clear();
            }
        }

        public void RunNextTask()
        {
            if (HasTask) return;

            if (_tasks.Any())
            {
                CurrentTask = _tasks.Dequeue();
                if (CurrentTask.ShouldStart())
                {
                    switch (CurrentTask)
                    {
                        case PickupPassengerTask pickup:
                            HandlePickupPassengerTask(pickup);
                            break;
                        case DespawnTask despawn:
                            HandleDespawnTask(despawn);
                            break;
                        case NeutralPathingTask pathing:
                            HandlePathingTask(pathing);
                            break;
                        case DropoffPassengerTask dropoff:
                            HandleDropoffTask(dropoff);
                            break;
                        default:
                            Debug.LogError($"No behaviour installed for vehicle for {CurrentTask.GetType().Name}");
                            break;
                    }
                }
                else
                {
                    // Task was skipped
                    CurrentTask = null;
                    RunNextTask();
                }
            }
            else
            {
                CurrentTask = null;
                Controller.IdleVehicle(this);
            }
        }

        internal void playSound()
        {
            if(this.AudioSource != null)
            {
                AudioSource.Play();
            }
        }

        private void HandleDropoffTask(DropoffPassengerTask dropoff)
        {
            FindPath(dropoff.TargetPassenger.DestRoute);
        }

        private void HandlePathingTask(NeutralPathingTask pathing)
        {
            StartPathing(pathing.Path);
        }

        private void HandleDespawnTask(DespawnTask despawn)
        {
            FindPath(despawn.TargetRoute);
        }

        private void HandlePickupPassengerTask(PickupPassengerTask task)
        {
            FindPath(task.TargetPassenger.StartRoute);
        }

        private void CompleteTask()
        {
            CurrentTask.Complete();
            CurrentTask = null;
            RunNextTask();
        }

        /// <summary>
        /// Immediately terminates all tasks for this vehicle
        /// </summary>
        public void HaltAllTasks()
        {
            _tasks.Clear();
            if (CurrentTask != null)
            {
                StopTraveling();
                CurrentTask = null;
            }
        }

        #endregion

        #region Passengers

        public void AddPassenger(Passenger passenger)
        {
            Passengers.Add(passenger);
            passenger.transform.SetParent(transform, false);
            AddTask(new DropoffPassengerTask(this, passenger));
        }

        public void RemovePassenger(Passenger passenger)
        {
            Debug.Assert(Passengers.Contains(passenger), "Passenger is not in vehicle???");
            Passengers.Remove(passenger);
        }

        public bool HasPassenger(Passenger passenger)
        {
            return Passengers.Contains(passenger);
        }

        #endregion

        private void GameStateChanged(GameEvent @event)
        {
            if (GameManager.CurrentGameState == GameState.LevelSimulating)
            {
                _startingPos = transform.position;
                _startingRot = transform.rotation;
            }
        }

        private void Reset(GameEvent @event)
        {
            HaltAllTasks();
            if (HasPassengers)
            {
                foreach (var passenger in Passengers)
                {
                    Destroy(passenger);
                }
                Passengers = new HashSet<Passenger>();
            }
            transform.position = _startingPos;
            transform.rotation = _startingRot;
        }

        #region VEHICLE PATHING

        public void FindPath(Route destinationRoute)
        {
            if (PathfindingManager.Instance.GetPath(CurrentRoute, destinationRoute, out var connections))
            {
                if (connections.Any())
                {
                    StartPathing(connections);
                }
                else
                {
                    // We are already at our destination
                    PathCompletionPercent = 1;
                }
            }
        }

        private void StartPathing(Queue<Connection> connections)
        {
            PathCompletionPercent = 0;
            VehiclePath = PathfindingManager.Instance.GenerateCurves(connections);
            NextPosition = VehiclePath.GetPointAt(0);

            // Green if has passenger else purple
            //            VehiclePathLine.colorGradient = HasPassengers ? DropoffGradient : PickupGradient;
            VehiclePathLine.positionCount = 20;
            if (!NeutralControlled) DrawPath(VehiclePath, VehiclePathLine);

            _canMove = true;
        }

        /// <summary>
        /// Halts this vehicle's pathing immediately (if any is being done)
        /// </summary>
        private void StopTraveling()
        {
            _canMove = false;
        }

        public void Move()
        {
            var t = transform;
            var targetPosition = Vector3.MoveTowards(t.position, NextPosition, Speed * Time.deltaTime);
            var difference = NextPosition - t.position;

            if (difference.magnitude > Granularity)
            {
                var targetRotation = Quaternion.LookRotation(difference);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }

            t.position = targetPosition;

            if (Vector3.Distance(t.position, NextPosition) < Granularity)
            {
                PathCompletionPercent = PathCompletionPercent + Granularity * 2;
                NextPosition = VehiclePath.GetPointAt(PathCompletionPercent);
                CurrentRoute = VehiclePath.GetNearestPoint(PathCompletionPercent)?.Route;

                if (!NeutralControlled) DrawPath(VehiclePath, VehiclePathLine);
            }
        }

        private void DrawPath(BezierCurve vehicleCurve, LineRenderer travelLine)
        {
            PathfindingManager.Instance.DrawCurve(vehicleCurve, travelLine, PathCompletionPercent, PathCompletionPercent + .25f);
        }

        #endregion

        #region Ring

        public void ActivateRing()
        {
            Debug.Log("RING SET ACTIVE");
            _ring.SetActive(true);
        }

        public void DeactivateRing()
        {
            _ring.SetActive(false);
        }


        private GameObject SpawnRing(Color color, float speed)
        {
            GameObject spawnedObj = Instantiate(RingPrefab, transform, false);
            Material ringMat = spawnedObj.GetComponent<Renderer>().material;
            ringMat.SetColor("_Color", color);
            ringMat.SetFloat("_Speed", speed);
            return spawnedObj;
        }

        private void ChangeRingColor(GameObject hoverobject)
        {
            if (hoverobject == this.gameObject)
            {

                _ring.GetComponent<Renderer>().material.SetColor("_Color", Color.magenta);
                _ring.GetComponent<Renderer>().material.SetFloat("_Speed", ringPulseSpeedOnHover);
            }
            else
            {
                _ring.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                _ring.GetComponent<Renderer>().material.SetFloat("_Speed", 2);
            }
        }

        #endregion

        public void Despawn()
        {
            Destroy(gameObject);
        }
    }
}