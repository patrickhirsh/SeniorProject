using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousDrag : MonoBehaviour {

    // Use this for initialization
    public float Distance;

    private void OnMouseDrag()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Distance);
        Vector3 obj = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = obj;
    }
}
