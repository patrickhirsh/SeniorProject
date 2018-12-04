using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARInputManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]

    public Camera Camera;
    public PlayerVehicleManager PlayerVehicleManager;
    public ARPlaneManager RPlaneManager;
    private bool _placed;
    private bool _vehicleSelected;
    public float AbovePlane;

    public GameObject Level;

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
                    var position = new Vector3(hitPose.position.x, hitPose.position.y + AbovePlane, hitPose.position.z);
                    SetupLevel(position);

                    TurnOffDebugPlanes();
                }
            }
        }
    }



    private void HandleMobileInput()
    {
        Debug.Log("Got some input 2");
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //If the level has been placed
            if (_placed)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    RaycastHit hitInfo;
                    //If raycast hits an object
                    var ray = Camera.ScreenPointToRay(touch.position);
                    Debug.DrawRay(ray.origin, ray.direction, Color.green, 20);

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        PlayerVehicleManager.HandleHit(hitInfo);
                    }
                    else
                    {
                        PlayerVehicleManager.HandleNotHit();
                    }
                }

            }
            //If the level hasn't been placed
            else
            {
                if (SessionOrigin.Raycast(touch.position, _sHits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = _sHits[0].pose;
                    var position = new Vector3(hitPose.position.x, hitPose.position.y + AbovePlane, hitPose.position.z);
                    SetupLevel(position);

                    TurnOffDebugPlanes();
                }
            }
        }
    }

    private void SetupLevel(Vector3 position)
    {
        Level.transform.position = position;
        //                    EntityManager.Instance.SpawnLevel(new Vector3(hitPose.position.x, hitPose.position.y + AbovePlane, hitPose.position.z));
        //m_SessionOrigin.gameObject.GetComponent<ARPlaneManager>().enabled = false;
        _placed = true;
    }

    private void TurnOffDebugPlanes()
    {
        ARPlaneMeshVisualizer[] toDisable = SessionOrigin.trackablesParent.GetComponentsInChildren<ARPlaneMeshVisualizer>();
        foreach (ARPlaneMeshVisualizer x in toDisable)
        {
            x.gameObject.SetActive(false);
        }
        //SessionOrigin.GetComponent<ARPointCloud>().gameObject.SetActive(false);
        _placed = true;
    }
}