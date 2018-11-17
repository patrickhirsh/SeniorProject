using Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Singleton
    private static InputManager _instance;
    public static InputManager Instance => _instance ?? (_instance = Create());

    private static InputManager Create()
    {
        var singleton = FindObjectOfType<InputManager>()?.gameObject;
        if (singleton == null) singleton = new GameObject { name = typeof(InputManager).Name };
        singleton.AddComponent<InputManager>();
        return singleton.GetComponent<InputManager>();
    }
    #endregion


   
}