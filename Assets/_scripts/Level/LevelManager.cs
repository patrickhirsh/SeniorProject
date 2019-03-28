using Level;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages construction of the level and high level state
/// </summary>
public class LevelManager : MonoBehaviour
{
    public GameObject LevelObject;

    private Dictionary<Building.BuildingColors, List<Route>> BuildingDict;

    #region Singleton
    private static LevelManager _instance;
    public static LevelManager Instance => _instance ?? (_instance = Create());

    private static LevelManager Create()
    {
        GameObject singleton = FindObjectOfType<LevelManager>()?.gameObject;
        if (singleton == null)
        {
            singleton = new GameObject { name = $"[{typeof(LevelManager).Name}]" };
            singleton.AddComponent<LevelManager>();
        }
        return singleton.GetComponent<LevelManager>();
    }
    #endregion


    #region Unity Methods

    private void Start()
    {
        //I put building management in here because passengers won't start() before the levelmanager does, and the data they need available to them will need to be available immediately. 
        BuildingDict = new Dictionary<Building.BuildingColors, List<Route>>();
        var buildings = FindObjectsOfType<Building>();
        foreach (Building x in buildings)
        {
            if (BuildingDict.ContainsKey(x.BuildingColor))
                BuildingDict[x.BuildingColor].Add(x.DeliveryLocation);
            else
                BuildingDict.Add(x.BuildingColor, new List<Route> { x.DeliveryLocation });
        }

        LevelObject = GameObject.Find("[LEVEL]");
    }

    #endregion

    public void Initialize()
    {
        EntityManager.Instance.Initialize();
    }

    public void GenerateLevel(LevelData data)
    {
        throw new NotImplementedException();
    }

    public void SaveLevel(LevelData data)
    {
        throw new NotImplementedException();
        // var json = JsonUtility.ToJson(data);
    }

    public void LoadLevel(LevelData data)
    {
        var level = JsonUtility.FromJson<LevelData>("{}");
        GenerateLevel(level);
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

    public void TransitionLevel(MenuBuilding newLevel)
    {
        //Destroy old level object
        Transform oldTransform = LevelObject.transform;
        Destroy(LevelObject);
        //Need to reset passenger manager
        ScoreManagerScript SM = GameObject.FindObjectOfType<ScoreManagerScript>();
        SM.SetPassengerSpecs(newLevel.passengerSpecs);

        PassengerManager PM = GameObject.FindObjectOfType<PassengerManager>();
        //Switch music to music of new level
        Osborne_AudioManager AM = GameObject.FindObjectOfType<Osborne_AudioManager>();
        AM.SwitchLevels(newLevel.NewLayer1, newLevel.NewLayer2, newLevel.NewLayer3);
        //Spawn and place new level prefab in that spot.
        LevelObject = Instantiate(newLevel.RepresentedLevel, oldTransform.position, oldTransform.rotation);
        
    }

}
