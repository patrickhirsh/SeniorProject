using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pin : MonoBehaviour
{
    public Camera cam;
    // Use this for initialization
    void Start()
    {
        //this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -3, 0);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

      transform.LookAt(cam.transform);

    }
}
