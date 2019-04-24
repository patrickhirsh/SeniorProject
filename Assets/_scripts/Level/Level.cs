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
        public ScoreController ScoreController;
        public GameObject ArtContainer;
        public GameObject menubuildingprefab;
        public CameraBubble Bubble;

#if UNITY_EDITOR
        public void Bake()
        {
            UnityEditor.Undo.RecordObject(this, "Bake Level");

            PlayerVehicleController = GetComponentInChildren<PlayerVehicleController>(true);
            NeutralVehicleController = GetComponentInChildren<NeutralVehicleController>(true);
            EnemyVehicleController = GetComponentInChildren<EnemyVehicleController>(true);
            EntityController = GetComponentInChildren<EntityController>(true);
            PassengerController = GetComponentInChildren<PassengerController>(true);
            ScoreController = GetComponentInChildren<ScoreController>(true);
            Bubble = GetComponentInChildren<CameraBubble>(true);

            CheckLevelSetup();

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
        private void CheckLevelSetup()
        {
            Debug.Assert(PlayerVehicleController != null, "Missing a Player Vehicle Controller");
            Debug.Assert(NeutralVehicleController != null, "Missing a Neutral Vehicle Controller");
            Debug.Assert(EnemyVehicleController != null, "Missing an Enemy Vehicle Controller");
            Debug.Assert(EntityController != null, "Missing an Entity Controller");
            Debug.Assert(PassengerController != null, "Missing a Passenger Controller");
            Debug.Assert(ScoreController != null, "Missing a Score Controller");
            Debug.Assert(Bubble != null, "Missing a Camera Bubble");
        }

        private void Awake()
        {
            CheckLevelSetup();
        }

        public void SetArtActive(bool b)
        {
            ArtContainer.SetActive(b);
        }

        public void Initialize()
        {
            // Start of Level
            EntityController.Initialize();
            PassengerController.Initialize();
            NeutralVehicleController.Initialize();
            ScoreController.Initialize();
            Osborne_AudioManager.Instance.SetLayers(true, false, false);
        }
    }
}