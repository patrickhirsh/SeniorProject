using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    /// <summary>
    /// Location that vehicles will deliver passengers to
    /// </summary>
    public Level.Route DeliveryLocation;

    public Material Red;
    public Material Blue;
    public Material Green;
    public Material Yellow;
    public Material Purple;
    public Material Orange;


    public enum BuildingColors {Red, Green, Blue, Yellow, Purple, Orange }

    public BuildingColors BuildingColor;

	// Use this for initialization
	void Start () {
        switch(BuildingColor)
        {
            case BuildingColors.Red:
                this.gameObject.GetComponent<Renderer>().material = Red;
                break;
            case BuildingColors.Green:
                this.gameObject.GetComponent<Renderer>().material = Green;
                break;
            case BuildingColors.Blue:
                this.gameObject.GetComponent<Renderer>().material = Blue;
                break;
            case BuildingColors.Yellow:
                this.gameObject.GetComponent<Renderer>().material = Yellow;
                break;
            case BuildingColors.Purple:
                this.gameObject.GetComponent<Renderer>().material = Purple;
                break;
            case BuildingColors.Orange:
                this.gameObject.GetComponent<Renderer>().material = Orange;
                break;
            default:

                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}




}
