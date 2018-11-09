using System;
using UnityEngine;
using System.Collections.Generic;
using Level;
using UnityEditor;
using Utility;

/// <summary>
/// Manages construction of the level and high level state
/// </summary>
public class LevelManager : MonoBehaviour
{
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
        EntityManager.Instance.Setup();
    }

    #endregion


    public void GenerateLevel(LevelData data)
    {
        throw new NotImplementedException();
    }

    public void SaveLevel(LevelData data)
    {
        var json = JsonUtility.ToJson(data);
        //TODO: Write to file
    }

    public void LoadLevel(LevelData data)
    {
        var level = JsonUtility.FromJson<LevelData>("{}");
        GenerateLevel(level);
    }
}
