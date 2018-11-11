using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SceneryContentPopulation : MonoBehaviour {

    public GameObject content;
    public GameObject buttonPrefab;

	// Use this for initialization
	void Start () {
        Debug.Log("Hit this");
        DirectoryInfo dir = new DirectoryInfo("Assets/_prefabs/Roads");
        FileInfo[] info = dir.GetFiles("*.prefab");
        
        foreach(FileInfo f in info)
        {
            
            DynamicButtonScript NewButton = Instantiate(buttonPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, content.transform).GetComponent<DynamicButtonScript>();
            
            NewButton.fileName = f.FullName;
            
            NewButton.setText(f.Name);

            Debug.Log("Assets\\_prefabs\\Roads\\" + f.Name);

            NewButton.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>NewButton.spawnTile((GameObject)Resources.Load("Assets\\_prefabs\\Roads\\" + f.Name)));
                
            Debug.Log("The file is: " + NewButton.fileName);
        }


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
