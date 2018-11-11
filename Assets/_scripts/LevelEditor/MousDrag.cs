using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousDrag : MonoBehaviour {

    // Use this for initialization
    public float distance;

    private void OnMouseDrag()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 Obj = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = Obj;
    }
}
