using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RideShareLevel
{
    public class Level : MonoBehaviour
    {
        public PlayerVehicleController PlayerVehicleController;
        public NeutralVehicleController NeutralVehicleController;
        public EnemyVehicleController EnemyVehicleController;
        public EntityController EntityController;
        public PassengerController PassengerController;

        public List<PassengerTypes> PassengerSpecs;


#if UNITY_EDITOR
        public void Bake()
        {
            UnityEditor.Undo.RecordObject(this, "Bake Level");

            PlayerVehicleController = GetComponentInChildren<PlayerVehicleController>();
            NeutralVehicleController = GetComponentInChildren<NeutralVehicleController>();
            EnemyVehicleController = GetComponentInChildren<EnemyVehicleController>();
            EntityController = GetComponentInChildren<EntityController>();
            PassengerController = GetComponentInChildren<PassengerController>();

            Debug.Assert(PlayerVehicleController != null, "Missing a Player Vehicle Controller");
            Debug.Assert(NeutralVehicleController != null, "Missing a Neutral Vehicle Controller");
            Debug.Assert(EnemyVehicleController != null, "Missing an Enemy Vehicle Controller");
            Debug.Assert(EntityController != null, "Missing an Entity Controller");
            Debug.Assert(PassengerController != null, "Missing a Passenger Controller");

            EntityController.Bake();
            PlayerVehicleController.Bake(this);
            PassengerController.Bake();
            foreach (var levelObject in GetComponentsInChildren<LevelObject>())
            {
                levelObject.SetLevel(this);
            }

            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif

        private void Awake()
        {
            // Start of Level
            EntityController.Initialize();
            PassengerController.Initialize();
            NeutralVehicleController.Initialize();
        }
    }
}