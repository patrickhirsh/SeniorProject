using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorController : MonoBehaviour {

    public int Overlapping;
    public Canvas WarningCanvas;

	// Use this for initialization
	void Start () {
        Overlapping = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Overlapping == 0)
        {
            WarningCanvas.gameObject.SetActive(false);
        }
        else
        {
            WarningCanvas.gameObject.SetActive(true);
        }
	}
}
