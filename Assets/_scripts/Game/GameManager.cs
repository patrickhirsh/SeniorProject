using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the state of the game like transition between levels or menus
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance ?? (_instance = Create());

    private static GameManager Create()
    {
        GameObject singleton = FindObjectOfType<GameManager>()?.gameObject;
        if (singleton == null) singleton = new GameObject { name = typeof(GameManager).Name };
        singleton.AddComponent<GameManager>();
        return singleton.GetComponent<GameManager>();
    }
    #endregion

}
