/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectSpawner : MonoBehaviour
{

    ARPlaneManager PlaneManager;
    public GameObject PassengerPrefab;

    // Use this for initialization
    void Start()
    {
        PlaneManager = gameObject.GetComponent<ARPlaneManager>();
        PlaneManager.planeAdded += OnPlaneAdded;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlaneAdded(ARPlaneAddedEventArgs args)
    {
        Debug.Log(args.plane.boundedPlane.Center);

        Instantiate(PassengerPrefab, args.plane.boundedPlane.Center, new Quaternion(0, 0, 0, 1));
    }
}*/
