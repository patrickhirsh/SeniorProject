using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class InputController : MonoBehaviour {

    private GameObject selected;
    private bool objectSelected;

    //Buttons
    public Button prefabButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        handleInput();
        if (objectSelected)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.x);
            mousePosition.y = 0;
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            selected.transform.position = objPosition;
        }
	}

    private void handleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click");
            RaycastHit hitInfo;
            bool rc = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            Debug.Log(rc);
            if (rc)
            {
                Debug.Log("Hit Object");
                selected = hitInfo.transform.gameObject;
                objectSelected = true;
            }
        }
    }
}
