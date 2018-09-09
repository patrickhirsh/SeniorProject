using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    [SerializeField]
    public List<Node> Connections;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            Gizmos.DrawLine(transform.position, Connections[i].transform.position);
        }
    }
}
