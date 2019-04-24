using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class InputManager : Singleton<InputManager>
{
    private Camera _camera;
    protected Camera MainCamera => _camera ? _camera : _camera = Camera.main;

    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    public GameObject Level;
    public ARSessionOrigin SessionOrigin;
    public float RaycastThickness;

    private bool _vehicleSelected;
    public LayerMask ColliderMask;

    private void Awake()
    {
        Broadcaster.AddListener(GameEvent.GameStateChanged, GameStateChanged);
    }

    private void GameStateChanged(GameEvent state)
    {
        switch (GameManager.CurrentGameState)
        {
            case GameState.LevelPlacement:
            case GameState.LevelRePlacement:
#if !PLATFORM_STANDALONE
                SetDebugPlanesActive(true);
#endif
                break;
            case GameState.LevelSimulating:
#if !PLATFORM_STANDALONE
                SetDebugPlanesActive(false);
#endif
                break;
            case GameState.GameEndManu:

                break;
        }
    }

    private void Update()
    {
#if (UNITY_ANDROID || UNITY_IOS) // && !UNITY_EDITOR
        HandleMobileInput();
#else
        HandleDesktopInput();
#endif
    }

#region Hover
    [Serializable]
    public class HoverEvent : UnityEvent<GameObject> { }
    public HoverEvent HoverChanged = new HoverEvent();

    public void HandleHover(bool hit, RaycastHit hitInfo)
    {
        if (hit)
        {
            HoverChanged?.Invoke(hitInfo.transform.gameObject);
        }
        else
        {
            HoverChanged.Invoke(null);
        }
    }
#endregion

#region Hit
    [Serializable]
    public class HitEvent : UnityEvent<GameObject> { }
    public HitEvent Hit = new HitEvent();
    public HitEvent NoHit = new HitEvent();

    private GameObject LastHit;

    public void HandleHit(RaycastHit hitInfo)
    {
        Hit?.Invoke(hitInfo.transform.gameObject);
        LastHit = hitInfo.transform.gameObject;
    }

    public void HandleNoHit(RaycastHit hitInfo)
    {
        NoHit?.Invoke(LastHit);
    }
#endregion

    private void HandleLevelSimulating(bool click)
    {
        if (!MainCamera) return;
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        var hit = Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, ColliderMask);

        HandleHover(hit, hitInfo);

        if (click && hit)
        {
            HandleHit(hitInfo);
        }
        else if (click)
        {
            HandleNoHit(hitInfo);
        }

    }

    private void HandleDesktopInput()
    {
        switch (GameManager.CurrentGameState)
        {
            case GameState.LevelPlaced:
            case GameState.LevelPlacement:
            case GameState.LevelRePlacement:
                if (Input.GetMouseButton(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject()) return;
                    List<ARRaycastHit> hits = new List<ARRaycastHit>();

                    if (SessionOrigin.Raycast(Input.mousePosition, hits, TrackableType.PlaneWithinPolygon))
                    {
                        Pose hitPose = hits[0].pose;
                        MoveLevel(hitPose.position);
                        if (GameManager.CurrentGameState == GameState.LevelPlacement)
                        {
                            GameManager.SetGameState(GameState.LevelPlaced);
                            ARScaler.Instance.ScaleOnPlacement();
                        }
                    }
                }
                break;
            case GameState.LevelSimulating:
                HandleLevelSimulating(Input.GetMouseButtonDown(0));
                break;
        }
    }

    private void HandleMobileInput()
    {
        Touch touch = new Touch();
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
        }

        switch (GameManager.CurrentGameState)
        {
            case GameState.LevelPlaced:
            case GameState.LevelPlacement:
            case GameState.LevelRePlacement:
                if (Input.touchCount > 0)
                {
                    var hits = new List<ARRaycastHit>();
                    if (SessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                    {
                        var hitPose = hits[0].pose;
                        MoveLevel(hitPose.position);
                        if (GameManager.CurrentGameState == GameState.LevelPlacement)
                        {
                            GameManager.SetGameState(GameState.LevelPlaced);
                            ARScaler.Instance.ScaleOnPlacement();
                        }
                    }
                }
                break;
            case GameState.LevelSimulating:
                HandleLevelSimulating(Input.touchCount > 0 && touch.phase == TouchPhase.Began);
                break;
            case GameState.GameEndManu:
                //Might not have to do anything here
                break;
        }
    }

    private void RotateLevel(Quaternion rotation)
    {
        Level.transform.rotation = rotation;
        ARScaler.Instance.SetRotation(rotation.y);
    }

    private void MoveLevel(Vector3 position)
    {
        LevelManager.Instance.CurrentLevel.transform.position = position;
    }

    private void SetDebugPlanesActive(bool active)
    {
        var visualizers = SessionOrigin.trackablesParent.GetComponentsInChildren<ARPlaneMeshVisualizer>(true);
        foreach (var x in visualizers)
        {
            x.gameObject.SetActive(active);
        }
        if (!active)
            SessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        //SessionOrigin.GetComponent<ARPointCloud>().gameObject.SetActive(false);
    }

    //Possible function to use?
    //public void StopPlaneDetection()
    //{
    //    ARSession session = ARSession.GetARSessionNativeInterface();
    //    ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
    //    config.planeDetection = UnityARPlaneDetection.None;
    //    session.RunWithConfig(config);
    //}
}