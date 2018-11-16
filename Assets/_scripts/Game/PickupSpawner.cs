using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;

public class PickupSpawner : MonoBehaviour {

    private List<PickupLocation> _Pickups;

    public GameObject SpawnPrefab;

    public float SpawnTime = 5.0f;
    private float _Timer;

	// Use this for initialization
	void Start () {
        //Set the timer
        _Timer = SpawnTime;

        // Get a list of all scene connections
        Connection[] connections = FindObjectsOfType<Connection>();

        _Pickups = new List<PickupLocation>();

        // Populate the Pickups list with every pickup in the scene
        foreach(Connection connection in connections)
        {
            foreach(PickupLocation location in connection.PickupLocations)
            {
                _Pickups.Add(location);
            }
        }

	}
	
	// Update is called once per frame
	void Update () {
        _Timer -= Time.deltaTime;

        if(_Timer <= 0)
        {
            SpawnObject();
            _Timer = SpawnTime;
        }
	}

    private void SpawnObject()
    {
        int id = Random.Range((int)0, _Pickups.Count);

        Instantiate(SpawnPrefab, _Pickups[id].transform.position, Quaternion.identity);
    }
}
