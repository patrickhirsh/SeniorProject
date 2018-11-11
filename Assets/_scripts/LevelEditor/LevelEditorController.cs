using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorController : MonoBehaviour {

    public int Overlapping;
    public Canvas warningCanvas;

	// Use this for initialization
	void Start () {
        Overlapping = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Overlapping == 0)
        {
            warningCanvas.gameObject.SetActive(false);
        }
        else
        {
            warningCanvas.gameObject.SetActive(true);
        }
	}
}
