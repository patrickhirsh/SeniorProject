using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicButtonScript : MonoBehaviour {

    public Canvas pMenu;

    public string fileName;

	// Use this for initialization
	void Start (string input) {
        pMenu = GameObject.Find("PrefabMenu").GetComponent<Canvas>();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void spawnTile(GameObject prefab)
    {
        //Close the prefab menu
        pMenu.gameObject.SetActive(false);
        //Spawn a tile
        Instantiate(prefab, new Vector3(0,0,0), new Quaternion(0,0,0,0));
    }

    internal void setText(string name)
    {
        Debug.Log("Setting text to" + name);
        this.GetComponentInChildren<UnityEngine.UI.Text>().text = name;
        Debug.Log("Finished");
    }

   
}
