using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownBehavior : MonoBehaviour {

    // The front of the vehicle
    [SerializeField]
    private Transform Front;

    // How far to cast in front of this object
    [SerializeField]
    private float CastDistance = 1.0f;

    private float RunningVelocity = 1.0f;

    private float Velocity = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // Cast a ray in front of this object
        RaycastHit hit;
        Physics.Raycast(Front.position, Front.forward, out hit, CastDistance);

        // If there was a hit
        if (hit.point != Vector3.zero)
        {
            float dist = Vector3.Distance(Front.position, hit.point);

            float unitDistance = dist / CastDistance;

            Velocity = Mathf.Lerp(Velocity, 0, 1 - unitDistance)
        }
        

	}
}
