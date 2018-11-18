using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
public class ARInputManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]

    public PlayerVehicleManager playerVehicleManager;
    public GameManager gameManager;


    GameObject m_PlacedPrefab;
    public ARPlaneManager RPlaneMan, ager;
    public GameObject prefab2;
    private bool placed;
    private bool VehicleSelected;
    public float abovePlane;
    public float scaleFloat;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    ARSessionOrigin m_SessionOrigin;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private void Awake()
    {

        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //If the level has been placed
            if (placed)
            {
                RaycastHit hitInfo;
                //If raycast hits an object
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hitInfo))
                {
                    playerVehicleManager.TakeInput(hitInfo);
                }
            }
            //If the level hasn't been placed
            else
            {
                if (m_SessionOrigin.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = s_Hits[0].pose;
                    spawnedObject = Instantiate(m_PlacedPrefab, new Vector3(hitPose.position.x, hitPose.position.y + abovePlane, hitPose.position.z), hitPose.rotation);
                    spawnedObject.transform.localScale = new Vector3(scaleFloat, scaleFloat, scaleFloat);
                    //m_SessionOrigin.gameObject.GetComponent<ARPlaneManager>().enabled = false;
                    placed = true;
                }
            }
        }
    }
}