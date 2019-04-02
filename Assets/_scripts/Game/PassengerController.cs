using System;
using System.Collections;
using System.Collections.Generic;
using RideShareLevel;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PassengerController : LevelObject
{
    public Passenger PassengerPrefab;
    public List<PassengerTypes> PassengerSpecs;

    private Terminal[] _terminals;

    private Dictionary<Building.BuildingColors, int> _spawned;
    private Dictionary<Building.BuildingColors, int> _delivered;
    private Dictionary<Building.BuildingColors, int> _playerDelivered;
    private Dictionary<Building.BuildingColors, PassengerTypes> _types;
    private Dictionary<Building.BuildingColors, float> _spawnTimer;
    private Dictionary<Building.BuildingColors, List<Route>> _buildingRoutes;

    private bool _canSpawn;

    private int PassengersInLevel => PassengerSpecs.Sum(types => types.NumSpawn);
    private int TotalDelivered => _delivered.Sum(pair => pair.Value);
    private int PlayerDelivered => _playerDelivered.Sum(pair => pair.Value);
    public int PassengersNeeded => PassengerSpecs.Sum(types => types.NumRequired);

    #region Unity Methods
    // Update is called once per frame
    private void Update()
    {
        if (!_canSpawn) return;

        foreach (var color in _spawnTimer.Keys.ToArray())
        {
            if (_spawned[color] < _types[color].NumSpawn)
            {
                _spawnTimer[color] -= Time.deltaTime;
                if (_spawnTimer[color] <= 0)
                {
                    SpawnPassenger(_types[color]);
                    _spawned[color]++;
                    _spawnTimer[color] = _types[color].TimeBetweenSpawn;
                }
            }
        }
    }

    #endregion

    private void SpawnPassenger(PassengerTypes spec)
    {

        var passenger = Instantiate(PassengerPrefab, transform, false);
        passenger.SetPassengerType(spec);

        StartCoroutine(FindTerminal(passenger));
    }

    private IEnumerator FindTerminal(Passenger passenger)
    {
        Terminal terminal = _terminals[Random.Range(0, _terminals.Length - 1)];
        int tries = 10;

        // keep trying to spawn a passenger until we find an empty terminal
        while (terminal.HasPassenger)
        {
            terminal = _terminals[Random.Range(0, _terminals.Length - 1)];
            tries--;
            if (tries <= 0)
            {
                yield return new WaitForSeconds(1);
            }
        }

        terminal.SetPassenger(passenger);
    }

    public void PassengerDelivered(Passenger passenger, bool byPlayer = false)
    {
        _delivered[passenger.GetColor()]++;
        if (byPlayer) _playerDelivered[passenger.GetColor()]++;
        Broadcaster.Broadcast(GameEvent.PassengerDelivered);

        if (TotalDelivered >= PassengersInLevel || PlayerDelivered >= PassengersNeeded)
        {
            Broadcaster.Broadcast(PlayerDelivered >= PassengersNeeded
                ? GameEvent.LevelCompleteSuccess
                : GameEvent.LevelCompleteFail);
        }
    }


#if UNITY_EDITOR
    public void Bake()
    {
        UnityEditor.Undo.RecordObject(this, "Bake Passenger Controller");
        var colors = PassengerSpecs.Select(types => types.PassColor);
        var buildingColors = CurrentLevel.GetComponentsInChildren<Building>().Select(building => building.BuildingColor).ToArray();

        foreach (var color in colors)
        {
            Debug.Assert(buildingColors.Contains(color), $"Could not find building for color {color}!");
        }

        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }
#endif

    public void Initialize()
    {
        // Populate the Pickups list with every pickup in the scene
        _terminals = CurrentLevel.EntityController.Routes.SelectMany(route => route.Terminals).ToArray();
        Debug.Assert(_terminals.Any(), "Missing terminals for the level. Has the EntityManager been baked?");

        _buildingRoutes = new Dictionary<Building.BuildingColors, List<Route>>();
        _delivered = new Dictionary<Building.BuildingColors, int>();
        _playerDelivered = new Dictionary<Building.BuildingColors, int>();
        _spawned = new Dictionary<Building.BuildingColors, int>();
        _spawnTimer = new Dictionary<Building.BuildingColors, float>();
        _types = new Dictionary<Building.BuildingColors, PassengerTypes>();

        foreach (var building in CurrentLevel.EntityController.Buildings)
        {
            if (_buildingRoutes.ContainsKey(building.BuildingColor))
                _buildingRoutes[building.BuildingColor].Add(building.DeliveryLocation);
            else
                _buildingRoutes.Add(building.BuildingColor, new List<Route> { building.DeliveryLocation });
        }

        foreach (var color in _buildingRoutes.Keys)
        {
            var spec = PassengerSpecs.FirstOrDefault(types => types.PassColor == color);
            Debug.Assert(spec != null, $"Could not find passenger type in {CurrentLevel.name} for {color}");

            _types[color] = spec;
            _spawnTimer[color] = spec.InitialDelay;
            _playerDelivered[color] = 0;
            _delivered[color] = 0;
            _spawned[color] = 0;
        }

        _canSpawn = true;
    }

    public Route GetBuildingRoute(Building.BuildingColors color)
    {
        if (_buildingRoutes[color].Any())
        {
            return _buildingRoutes[color].First();
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

    public IEnumerable<Building.BuildingColors> GetBuildingColors()
    {
        return _buildingRoutes.Keys;
    }

    public int GetPassengersNeeded(Building.BuildingColors color)
    {
        return _types[color].NumRequired - _playerDelivered[color];
    }

    public int GetPassengersRequired(Building.BuildingColors color)
    {
        return _types[color].NumRequired;
    }
}