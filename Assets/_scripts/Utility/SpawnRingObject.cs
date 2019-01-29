using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRingObject : MonoBehaviour {

    [SerializeField]
    public GameObject _RingObject;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject SpawnRing(Vector3 position, Color RingColor, float RingSpeed)
    {
        GameObject spawnedObj = Instantiate(_RingObject, position, Quaternion.identity);

        Material ringMat = spawnedObj.GetComponent<Renderer>().material;
        ringMat.SetColor("_Color", RingColor);
        ringMat.SetFloat("_Speed", RingSpeed);

        return spawnedObj;
    }
}
