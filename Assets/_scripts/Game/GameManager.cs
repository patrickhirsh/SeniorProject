using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the state of the game like transition between levels or menus
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public Material TempMaterial;
    public static GameManager Instance => _instance ?? (_instance = Create());

    public float Scale
    {
        get { return Application.isPlaying ? _scale : _scale = 1f; }
        private set { _scale = value; }
    }

    [Serializable]
    public class ScaleEvent : UnityEvent<float> { }
    public ScaleEvent OnScaleChangeEvent = new ScaleEvent();
    private float _scale;

    private static GameManager Create()
    {
        GameObject singleton = FindObjectOfType<GameManager>()?.gameObject;
        if (singleton == null)
        {
            singleton = new GameObject { name = $"[{typeof(GameManager).Name}]" };
            singleton.AddComponent<GameManager>();
        }
        return singleton.GetComponent<GameManager>();
    }
    #endregion

    private void Start()
    {
        LevelManager.Instance.Initialize();
    }

    public void SetScale(float scaleValue)
    {
        Scale = scaleValue;
        OnScaleChangeEvent?.Invoke(scaleValue);
    }
}
