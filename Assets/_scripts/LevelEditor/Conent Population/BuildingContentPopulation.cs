using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BuildingContentPopulation : MonoBehaviour {

    public GameObject Content;
    public GameObject ButtonPrefab;

	// Use this for initialization
	void Start () {
        Debug.Log("Hit this");
        DirectoryInfo dir = new DirectoryInfo("Assets/_prefabs/Buildings");
        FileInfo[] info = dir.GetFiles("*.prefab");
        
        foreach(FileInfo f in info)
        {
            
            DynamicButtonScript newButton = Instantiate(ButtonPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, Content.transform).GetComponent<DynamicButtonScript>();
            
            newButton.FileName = f.FullName;
            
            newButton.SetText(f.Name);

            Debug.Log("Assets\\_prefabs\\Buildings\\" + f.Name);

            newButton.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>newButton.SpawnTile((GameObject)Resources.Load("Assets\\_prefabs\\Buildings\\" + f.Name)));
                
            Debug.Log("The file is: " + newButton.FileName);
        }


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
