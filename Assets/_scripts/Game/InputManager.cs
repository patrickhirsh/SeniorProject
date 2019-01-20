using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]

    public Camera Camera;
    public GameObject Level;
    public ARSessionOrigin SessionOrigin;
    public float LevelYOffset;
    public PlayerVehicleManager PlayerVehicleManager;
    public float raycastThickness;

    private bool _vehicleSelected;

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
                SetDebugPlanesActive(true);
                break;
            case GameState.LevelSimulating:
                SetDebugPlanesActive(false);
                break;
        }
    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        HandleMobileInput();
#else
        HandleDesktopInput();
#endif
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
                        var position = new Vector3(hitPose.position.x, hitPose.position.y + LevelYOffset, hitPose.position.z);
                        MoveLevel(position);
                    }
                }
                break;
            case GameState.LevelSimulating:
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo;
                    //If raycast hits an object
                    Vector3 origin = Camera.main.transform.position;
                       
                    if (Physics.SphereCast(origin, raycastThickness, Camera.main.transform.forward, out hitInfo))
                    {
                        PlayerVehicleManager.HandleHit(hitInfo);
                    }
                    else
                    {
                        PlayerVehicleManager.HandleNotHit();
                    }
                }
                break;
        }
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount <= 0) return;
        var touch = Input.GetTouch(0);
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

        switch (GameManager.CurrentGameState)
        {
            case GameState.LevelPlaced:
            case GameState.LevelPlacement:
            case GameState.LevelRePlacement:
                var hits = new List<ARRaycastHit>();
                if (SessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    var position = new Vector3(hitPose.position.x, hitPose.position.y + LevelYOffset, hitPose.position.z);
                    MoveLevel(position);
                }
                break;
            case GameState.LevelSimulating:
                if (touch.phase == TouchPhase.Began)
                {
                    RaycastHit hitInfo;
                    Vector3 origin = Camera.main.transform.position;

                    if (Physics.SphereCast(origin, raycastThickness, Camera.main.transform.forward, out hitInfo))
                    {
                        PlayerVehicleManager.HandleHit(hitInfo);
                    }
                    else
                    {
                        PlayerVehicleManager.HandleNotHit();
                    }
                }
                break;
        }
    }

    private void MoveLevel(Vector3 position)
    {
        Level.transform.position = position;
        GameManager.SetGameState(GameState.LevelPlaced);
    }

    private void SetDebugPlanesActive(bool active)
    {
        var visualizers = SessionOrigin.trackablesParent.GetComponentsInChildren<ARPlaneMeshVisualizer>(true);
        foreach (var x in visualizers)
        {
            x.gameObject.SetActive(active);
        }
        //SessionOrigin.GetComponent<ARPointCloud>().gameObject.SetActive(false);
    }
}