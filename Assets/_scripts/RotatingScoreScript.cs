using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingScoreScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //this.transform.Rotate(new Vector3(0, 1, 0));
	}

    private void LateUpdate()
    {
        Vector3 v3T = transform.position + Camera.main.transform.rotation * Vector3.forward;
        v3T.y = transform.position.y;
        transform.LookAt(v3T, Vector3.up);
    }
}
