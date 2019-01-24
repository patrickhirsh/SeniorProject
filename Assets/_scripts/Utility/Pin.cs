using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;


public class Pin : MonoBehaviour
{
    public Passenger Passenger;
    public SpriteRenderer SpriteRenderer;

    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
