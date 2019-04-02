using System.Collections.Generic;
using RideShareLevel;
using System.Linq;
using UnityEngine;

public class PassengerController : LevelObject
{
    [SerializeField]
    [HideInInspector]
    private Building[] _buildings;

    private Terminal[] _terminals;
    
    public Passenger PassengerPrefab;
    private Dictionary<Building.BuildingColors, int> SpawnedDictionary;
    private Dictionary<Building.BuildingColors, List<Route>> BuildingDict;

    public static float PassengerTimeout = 60.0f;
    public float SpawnTime = 30.0f;
    public int PassengersToSpawn = 30;
    private int _passengerCount = 0;
    private int _passengersDelivered = 0;
    private float _timer;

    #region Unity Methods
    // Update is called once per frame
    private void Update()
    {
        if (GameManager.CurrentGameState == GameState.LevelSimulating)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                if (_passengerCount <= PassengersToSpawn)
                {
                    SpawnPassenger();
                    _passengerCount++;
                    _timer = SpawnTime;
                }            
            }
        }
    }

    #endregion

    private void SpawnPassenger()
    {
        int index = Random.Range(0, _terminals.Length - 1);

        // keep trying to spawn a passenger until we find an empty terminal
        while (!_terminals[index].SpawnPassenger(PassengerPrefab))
            index = Random.Range(0, _terminals.Length - 1);
    }

    public void PassengerDelivered(Passenger passenger)
    {
        _passengersDelivered++;
        if (_passengersDelivered == PassengersToSpawn)
        {
            Debug.Log("Passenger have all been delivered");
            Broadcaster.Broadcast(GameEvent.LevelComplete);
        }
    }


#if UNITY_EDITOR
    public void Bake()
    {
        UnityEditor.Undo.RecordObject(this, "Bake Passenger Controller");
        _buildings = GetComponentsInChildren<Building>();
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }
#endif

    public void Initialize()
    {
        // Populate the Pickups list with every pickup in the scene
        _terminals = CurrentLevel.EntityController.Routes.SelectMany(route => route.Terminals).ToArray();
        Debug.Assert(_terminals.Any(), "Missing terminals for the level. Has the EntityManager been baked?");

        BuildingDict = new Dictionary<Building.BuildingColors, List<Route>>();
        foreach (Building x in _buildings)
        {
            if (BuildingDict.ContainsKey(x.BuildingColor))
                BuildingDict[x.BuildingColor].Add(x.DeliveryLocation);
            else
                BuildingDict.Add(x.BuildingColor, new List<Route> { x.DeliveryLocation });
        }
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

    /// <summary>
    /// Get's a valid color for the current level. 
    /// </summary>
    /// <returns>A color that is assigned to a building</returns>
    public Building.BuildingColors GetValidColor()
    {
        var buildingColors = new List<Building.BuildingColors>(BuildingDict.Keys);
        return buildingColors[Random.Range(0, buildingColors.Count)];
    }
}