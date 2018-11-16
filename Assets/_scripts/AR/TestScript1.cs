using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class TestScript1 : MonoBehaviour {

    private bool placed;
    public ARSessionOrigin aRSessionOrigin;
    private List<ARRaycastHit> s_Hits;
    public GameObject spawnedObject;
    public GameObject prefab;

    // Use this for initialization
    void Start () {
        placed = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            RaycastHit hitInfo;

            if (!placed)
            {
                if (aRSessionOrigin.Raycast(touch.position, s_Hits,TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = s_Hits[0].pose;

                    if (spawnedObject == null)
                    {
                        spawnedObject = Instantiate(prefab, hitPose.position, hitPose.rotation);
                        placed = true;
                    }
                    else
                    {
                        //spawnedObject.transform.position = hitPose.position;
                    }
                }
            }
            else
            {
                //Do nothing
            }

        }
    }
}

