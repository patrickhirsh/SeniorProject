using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ArInputManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]

    public Camera Camera;
    public PlayerVehicleManager PlayerVehicleManager;
    public ARPlaneManager RPlaneManager;
    private bool _placed;
    private bool _vehicleSelected;
    public float AbovePlane;
    public float ScaleFloat;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject PlacedPrefab;

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject SpawnedObject { get; private set; }

    public ARSessionOrigin SessionOrigin;

    static List<ARRaycastHit> _sHits = new List<ARRaycastHit>();

    private void Awake()
    {
        _placed = false;
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
        if (Input.GetMouseButtonDown(0))
        {
            //If the level has been placed
            if (_placed)
            {
                RaycastHit hitInfo;
                //If raycast hits an object
                var ray = Camera.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction, Color.green, 20);
                Debug.Log("RAYCAST");
                if (Physics.Raycast(ray, out hitInfo))
                {
                    PlayerVehicleManager.HandleHit(hitInfo);
                }
                else
                {
                    PlayerVehicleManager.HandleNotHit();
                }
            }
            //If the level hasn't been placed
            else
            {
                if (SessionOrigin.Raycast(Input.mousePosition, _sHits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = _sHits[0].pose;
                    EntityManager.Instance.SpawnLevel(new Vector3(hitPose.position.x, hitPose.position.y + AbovePlane, hitPose.position.z), new Vector3(ScaleFloat, ScaleFloat, ScaleFloat));
                    //m_SessionOrigin.gameObject.GetComponent<ARPlaneManager>().enabled = false;
                    _placed = true;
                }
            }
        }
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("LETS GO");
            Touch touch = Input.GetTouch(0);
            //If the level has been placed
            if (_placed)
            {
                RaycastHit hitInfo;
                //If raycast hits an object
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hitInfo))
                {
                    PlayerVehicleManager.HandleHit(hitInfo);
                }
                else
                {
                    PlayerVehicleManager.HandleNotHit();
                }
            }
            //If the level hasn't been placed
            else
            {
                if (SessionOrigin.Raycast(touch.position, _sHits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = _sHits[0].pose;
                    EntityManager.Instance.SpawnLevel(new Vector3(hitPose.position.x, hitPose.position.y + AbovePlane, hitPose.position.z), new Vector3(ScaleFloat, ScaleFloat, ScaleFloat));
                    _placed = true;
                }
            }
        }
    }
}