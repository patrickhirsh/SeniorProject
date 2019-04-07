using UnityEngine;

namespace RideShareLevel
{
    /// <summary>
    /// Vehicle managers for all vehicle types should implement this class. 
    /// </summary>
    public abstract class VehicleController : LevelObject
    {
        [SerializeField]
        [ReadOnly]
        protected Vehicle[] Vehicles;

        #region Unity Methods

        private void Awake()
        {
            Broadcaster.AddListener(GameEvent.Reset, Reset);
            Vehicles = GetComponentsInChildren<Vehicle>();
        }
        #endregion

#if UNITY_EDITOR
        public void Bake(Level level)
        {
            UnityEditor.Undo.RecordObject(this, "Bake Vehicle Controller");
            Vehicles = GetComponentsInChildren<Vehicle>();
            foreach (var vehicle in Vehicles)
            {
                vehicle.Bake();
            }
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);

        }
#endif

        private void Reset(GameEvent @event)
        {
            Vehicles = GetComponentsInChildren<Vehicle>();
        }

        public abstract void IdleVehicle(Vehicle vehicle);
    }
}

