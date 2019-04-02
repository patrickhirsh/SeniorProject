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

        public List<PassengerTypes> PassengerSpecs;

        private Dictionary<Building.BuildingColors, List<Route>> BuildingDict;

#if UNITY_EDITOR
        public void Bake()
        {
            UnityEditor.Undo.RecordObject(this, "Bake Level");

            PlayerVehicleController = GetComponentInChildren<PlayerVehicleController>();
            NeutralVehicleController = GetComponentInChildren<NeutralVehicleController>();
            EnemyVehicleController = GetComponentInChildren<EnemyVehicleController>();
            EntityController = GetComponentInChildren<EntityController>();

            Debug.Assert(PlayerVehicleController != null, "Missing a Player Vehicle Controller");
            Debug.Assert(NeutralVehicleController != null, "Missing a Neutral Vehicle Controller");
            Debug.Assert(EnemyVehicleController != null, "Missing an Enemy Vehicle Controller");
            Debug.Assert(EntityController != null, "Missing an Entity Controller");

            EntityController.Bake();
            PlayerVehicleController.Bake(this);
            foreach (var levelObject in GetComponentsInChildren<LevelObject>())
            {
                levelObject.SetLevel(this);
            }

            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif
        /// <summary>
        /// Get's a valid color for the current level. 
        /// </summary>
        /// <returns>A color that is assigned to a building</returns>
        public Building.BuildingColors GetValidColor(Dictionary<Building.BuildingColors, int> valuePairs)
        {
            //Get all the colors of buildings in the level
            var buildingColors = new List<Building.BuildingColors>(BuildingDict.Keys);
            //New list for pool of valid colors to select from
            List<Building.BuildingColors> ColorPool = new List<Building.BuildingColors>();
            //Iterate through each buildings color
            foreach (Building.BuildingColors color in buildingColors)
            {
                //If the tracking dictionary contains the color
                if (valuePairs.ContainsKey(color))
                {
                    //And if there have not been too many of a given color passenger spawned
                    if (!(valuePairs[color] >= PassengerSpecs.Find(e => e.passColor == color).numSpawn))
                    {
                        //Add to pool of possible colors for spawned passenger
                        ColorPool.Add(color);
                    }
                }
                //If the tracking dictionary has not yet seen the color
                else
                {
                    //Add the color to the dictionary w
                    valuePairs.Add(color, 0);
                    //Allow color to be added too pool
                    ColorPool.Add(color);
                }
            }
            //Return a randomly selected color for the passenger
            return ColorPool[Random.Range(0, buildingColors.Count)];
        }

        public Route GetBuildingRoute(Building.BuildingColors color)
        {
            if (BuildingDict[color].Any())
            {
                return BuildingDict[color].First();
            }
            else
            {
                Debug.LogWarning($"No building for color {color}");
                //I added this in here in case we want to have multiple buildings of the same color, all we need to do is a distance formula
                //to find the closest building to the last person we pick up of whatever color
                //If we decide to do that I'll program in the distance thing later, that'll just be a bunch more work. 
                return null;
            }
        }

        private void Awake()
        {
            // Start of Level
            EntityController.Initialize();
            NeutralVehicleController.Initialize();

            //I put building management in here because passengers won't start() before the levelmanager does, and the data they need available to them will need to be available immediately. 
            BuildingDict = new Dictionary<Building.BuildingColors, List<Route>>();
            var buildings = GetComponentsInChildren<Building>();
            foreach (Building x in buildings)
            {
                if (BuildingDict.ContainsKey(x.BuildingColor))
                    BuildingDict[x.BuildingColor].Add(x.DeliveryLocation);
                else
                    BuildingDict.Add(x.BuildingColor, new List<Route> { x.DeliveryLocation });
            }
        }


    }
}