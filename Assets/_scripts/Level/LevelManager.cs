using Level;
using System;
using UnityEngine;

/// <summary>
/// Manages construction of the level and high level state
/// </summary>
public class LevelManager : MonoBehaviour
{

    System.Collections.Generic.Dictionary<Building.BuildingColors, System.Collections.Generic.List<Route>> buildingDict;

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
        buildingDict = new System.Collections.Generic.Dictionary<Building.BuildingColors, System.Collections.Generic.List<Route>>();
        var buildings = FindObjectsOfType<Building>();
        foreach(Building x in buildings)
        {
            if (buildingDict.ContainsKey(x.BuildingColor))
                buildingDict[x.BuildingColor].Add(x.DeliveryLocation);
            else
                buildingDict.Add(x.BuildingColor, new System.Collections.Generic.List<Route> { x.DeliveryLocation });
        }
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
        System.Random rand = new System.Random();
        var keylist = new System.Collections.Generic.List<Building.BuildingColors>(buildingDict.Keys);
        Debug.Log(keylist.Count);
        return keylist[rand.Next(0, keylist.Count)];
        
    }

    public Level.Route GetBuildingRoute(Building.BuildingColors color)
    {
        if(buildingDict[color].Count == 1)
        {
            return buildingDict[color][0];
        }
        else
        {
            //I added this in here in case we want to have multiple buildings of the same color, all we need to do is a distance formula
            //to find the closest building to the last person we pick up of whatever color
            //If we decide to do that I'll program in the distance thing later, that'll just be a bunch more work. 
            return null;
        }
    }

}
