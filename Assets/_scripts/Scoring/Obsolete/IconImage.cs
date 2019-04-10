using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
     //nothing   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void ChangeColr(Material InputMaterial)
    {
        this.gameObject.GetComponent<SpriteRenderer>().material = InputMaterial;
    }
}
