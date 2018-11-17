﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;
    public ARPlaneManager RPlaneManager;
    public GameObject prefab2;
    private bool placed;
    public float abovePlane;

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
    
    void Awake()
    {
        placed = false;
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (!placed)
            {
                if (m_SessionOrigin.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = s_Hits[0].pose;

                    if (spawnedObject == null)
                    {
                        spawnedObject = Instantiate(m_PlacedPrefab, new Vector3(hitPose.position.x, hitPose.position.y + abovePlane, hitPose.position.z), hitPose.rotation);
                        spawnedObject.transform.localScale = new Vector3(.1f, .1f, .1f);
                        //m_SessionOrigin.gameObject.GetComponent<ARPlaneManager>().enabled = false;
                        placed = true;
                        
                    }
                    else
                    {
                        spawnedObject.transform.position = hitPose.position;
                    }
                }
            }
        }
    }
}
