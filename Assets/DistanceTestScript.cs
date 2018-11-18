using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceTestScript : MonoBehaviour {

    public Level.Connection debug;
    public Level.Connection debug2;


    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("THE TEST: " + Vector3.Distance(debug.transform.position, debug2.transform.position));
    }
}
