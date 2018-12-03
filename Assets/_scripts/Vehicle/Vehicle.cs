﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Task type represent the precedence of a received task. Each type starting at the top and
    /// moving in descending order takes precedence over all tasks below it. (lower value = higher precedence).
    /// Typically, vehicles controlled by player/ai only receive tasks marked within their category, and neutral
    /// vehicles only ever receive a neutralPathing task. That said, this task precedence system allows for
    /// task assignment of any type to any vehicle.
    /// </summary>
    public enum TaskType
    {
        // PLAYER/AI VEHICLES
        ActivePlayer = 0,       // directive from a player system
        ActiveAi = 1,           // directive from an AI system
        PassivePlayer = 2,      // directive from player system with no current instructions
        PassiveAi = 3,          // directive from AI systems with no current instructions

        // NEUTRAL VEHICLES
        NeutralAi = 4           // directive from the neutralVehicleManager
    }

    /// <summary>
    /// Represents a "path" task of precedence "type" that can be assigned to a vehicle.
    /// "callback" is called after a task is completed
    /// </summary>
    public class VehicleTask
    {
        public TaskType Type { get; private set; }                  // task type determines vehicle control precedence
        public Queue<Connection> Path;                              // pathing directive for this task
        public System.Action<TaskType, Vehicle, bool> Callback;     // called upon task completiton (true) or task inturruption (false)

        public VehicleTask(TaskType type, Queue<Connection> path, System.Action<TaskType, Vehicle, bool> callback)
        {
            Type = type;
            Path = path;
            Callback = callback;
        }
    }

    /// <summary>
    /// Vehicle is a basic movement script that knows nothing about the objects controlling it.
    /// Vehicle is responsible for accepting tasks based on task precedence. Vehicle will
    /// immediately execute a task given to it that has a higher precedence than its current task. 
    /// All other lower precedence task assignments will be ignored while this task is being executed.
    /// 
    /// Typical Vehicle Control Usage:
    /// 
    /// - Vehicle defaults to a non-moving "waiting for instructions" state (null) on instantiation. It is
    /// the responsibility of the object that created this vehicle to assign it tasks
    /// - Active tasks should be assigned to inturupt passive tasks
    /// - Passive tasks should be assigned upon completion of an active task (by the respective manager)
    /// 
    /// </summary>
    public class Vehicle : MonoBehaviour
    {
        public Route CurrentRoute;              // the route this vehicle is currently on
        public Connection CurrentConnection;    // the connection this vehicle is currently on
        public float LookAhead = .2f;           // used to create more natural turn animations
        public float Speed = 5f;                // the speed at which this vehicle will traverse it's current path
        public float RecoverySpeed = 5f;        // the speed at which this vehicle will travel to recover when "lost"
        public float BaseSpeed = 5f;            // the speed this car will travel at its fastest

        private Coroutine _animationTween;      // this coroutine is executed during "travelling"
        private VehicleTask _currentTask;        // the highest-precedence task currently assigned to this vehicle. Determines the vehicle's behavior.

        public Passenger Passenger;
        public bool HasPassenger => Passenger != null;

        protected void Start()
        {
            /*
             * A WARNING TO ALL YE SORRY FOLK WHO WANT TO ADD ANYTHING TO VEHICLE.START()
             * 
             * For some reason, Start() is invoked multiple times throughout a Vehicle's task execution.
             * As you can see by the 0 references, it must be unity that does the multiple invokes... 
             * From what I can tell, it happens immediately after the TravelTo() coroutine completes
             * and immediatly before TravelPath() begins. I don't know why, but be warned. Until this
             * mystery is further explored, anything in this method CAN AND WILL be executed after each
             * coroutine is finished. No, the vehicle is not being re-instantiated/re-initialized. This is
             * a call to Start() on an exisiting and initialized Vehicle object. This can be verified by
             * placing a breakpoint here and observing the data fields that should be null on initialize 
             * (such as _currentTask) during the second call to Start().
             * 
             * TODO: Figure out what the hell is going on here
            */

            // Don't place anything here that can't safely be executed after the completion of any task coroutine...

            _animationTween = null;     
        }


        /// <summary>
        /// tries to assign a new task to the current vehicle.
        /// If the given task is at equal or higher precedence to the current task, the given task
        /// will take control of the vehicle (and this method returns true). Otherwise, the task
        /// is discarded and this method returns false.
        /// </summary>
        /// <returns></returns>
        public bool AssignTask(VehicleTask task)
        {
            /*
            // if a current connection wasn't assigned to this vehicle, find the nearest connection and assign it
            // this is somewhat expensive. Vehicles should have their starting connections assigned through Initialize() on instantiation
            if (CurrentConnection == null)
                CurrentConnection = EntityManager.Instance.Connections.Where(connection => connection.Paths.Any())
                    .OrderBy(connection => Vector3.Distance(transform.position, connection.transform.position))
                    .FirstOrDefault();
            */

            // check if the given task takes higher (or equal) precedence to the current task
            if ((_currentTask == null) || ((int)task.Type <= (int)_currentTask.Type))
            {
                // notify the task's source that the task was terminated prematurely
                if (_currentTask != null)
                    _currentTask.Callback(_currentTask.Type, this, false);

                // give control to the new task
                StopTraveling();
                _currentTask = task;
                StartTraveling(task.Path);
                Debug.Log(_currentTask.ToString());
                return true;
            }

            // the given task has lower precedence than the current task
            else { return false; }
        }


        /// <summary>
        /// Immediately terminates the current task, if one exists.
        /// </summary>
        public void HaltCurrentTask()
        {
            if (_currentTask != null)
            {
                StopTraveling();
                _currentTask = null;
            }
        }


        #region VEHICLE PATHING

        /// <summary>
        /// Halts this vehicle's pathing immediately (if any is being done)
        /// </summary>
        private void StopTraveling()
        {
            if (_animationTween != null)
            {
                StopCoroutine(_animationTween);
                CurrentConnection = null;
            }
        }

        /// <summary>
        /// Moves this vehicle along the given path of connections.
        /// traversing it by starting the TravelPath() coroutine.
        /// </summary>
        private void StartTraveling(Queue<Connection> connections)
        {
            Debug.Assert(connections.Any(), "No connections to path");

            // begin traveling the new path
            var firstConnection = connections.Peek();
            _animationTween = StartCoroutine(Travel(connections, firstConnection));
        }

        private IEnumerator Travel(Queue<Connection> connections, Connection firstConnection)
        {
            // if the last call to StartTraveling was interrupted, recover the vehicle
            if (CurrentConnection == null)
                yield return TravelTo(firstConnection);

            var vehicleCurve = PathfindingManager.Instance.GeneratePath(connections);
            Debug.Log(vehicleCurve.pointCount);

            yield return StartCoroutine(TravelPath(vehicleCurve));
        }



        /// <summary>
        /// A coroutine that moves the vehicle along a BezierCurve path.
        /// If the vehicle is in a "lost" state, it'll first resolve the
        /// lost vehicle before pathing.
        /// </summary>
        private IEnumerator TravelPath(BezierCurve vehicleCurve)
        {
            // Build a line to visualize on
            var travelLine = new GameObject("Line", typeof(LineRenderer)).GetComponent<LineRenderer>();
            int lengthOfLineRenderer = 20;
            travelLine.positionCount = lengthOfLineRenderer;
            travelLine.widthMultiplier = .15f;

            travelLine.numCapVertices = 2;
            travelLine.numCornerVertices = 2;
            travelLine.useWorldSpace = false;

            // traverse the path
            float position = 0.0f;
            while (position <= 1)
            {
                position += (Speed * Time.deltaTime) / vehicleCurve.length;
                transform.position = vehicleCurve.GetPointAt(Mathf.Clamp01(position));

                if (position + LookAhead < 1f)
                    transform.LookAt(vehicleCurve.GetPointAt(position + LookAhead));
                Debug.DrawLine(transform.position, vehicleCurve.GetPointAt(position + LookAhead), Color.cyan, .5f);
                PathfindingManager.Instance.DrawPath(vehicleCurve, travelLine);

                yield return null;
            }

            // Invoke callback and set vehicle back to "waiting for task" state
            _currentTask.Callback?.Invoke(_currentTask.Type, this, true);
            
            // Remove the curve
            Destroy(vehicleCurve);
            Destroy(travelLine.gameObject);
        }


        /// <summary>
        /// A coroutine that moves the vehicle towards a given connection.
        /// Useful for recovering a vehicle from a "lost" state by moving it
        /// to the first connection in a new given path.
        /// </summary>
        private IEnumerator TravelTo(Connection connection)
        {
            while (Vector3.Distance(transform.position, connection.transform.position) > .1f)
            {
                float step = RecoverySpeed * Time.deltaTime;

                Vector3 targetDir = connection.transform.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                
                // Move our position a step closer to the target.
                transform.rotation = Quaternion.LookRotation(newDir);

                transform.position = Vector3.MoveTowards(transform.position, connection.transform.position, step);
                yield return null;
            }
            CurrentConnection = connection;
        }

        #endregion

        public void SetCurrentRoute(Route route)
        {
            CurrentRoute = route;
        }

        public void AddPassenger(Passenger passenger)
        {
            Passenger = passenger;
            passenger.transform.parent = transform;
            passenger.transform.localPosition = Vector3.zero;
        }
    }
}