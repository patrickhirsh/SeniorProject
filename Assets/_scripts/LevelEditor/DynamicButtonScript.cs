using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicButtonScript : MonoBehaviour {

    public Canvas PMenu;

    public string FileName;

	// Use this for initialization
	//void Start (string input) {
 //       pMenu = GameObject.Find("PrefabMenu").GetComponent<Canvas>();
        
	//}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnTile(GameObject prefab)
    {
        //Close the prefab menu
        PMenu.gameObject.SetActive(false);
        //Spawn a tile
        Instantiate(prefab, new Vector3(0,0,0), new Quaternion(0,0,0,0));
    }

    internal void SetText(string name)
    {
        Debug.Log("Setting text to" + name);
        this.GetComponentInChildren<UnityEngine.UI.Text>().text = name;
        Debug.Log("Finished");
    }

   
}
