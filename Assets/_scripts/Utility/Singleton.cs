using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Singleton
    private static T _instance;
    public static T Instance => _instance != null ? _instance : _instance = Create();

    private static T Create()
    {
        GameObject singleton = (FindObjectOfType(typeof(T)) as T)?.gameObject;
        if (singleton == null)
        {
            singleton = new GameObject { name = $"[{typeof(T).Name}]" };
            singleton.AddComponent(typeof(T));
        }
        return singleton.GetComponent<T>();
    }
    #endregion
}