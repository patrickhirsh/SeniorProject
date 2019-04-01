using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBuilding : MonoBehaviour
{
    //Public field to place the level prefab that we want to load into the scene
    public GameObject RepresentedLevel;
    //Audio layers for level to be transitioned to.
    public AudioClip NewLayer1;
    public AudioClip NewLayer2;
    public AudioClip NewLayer3;
    //Passenger specs for specififed level
    public List<PassengerTypes> passengerSpecs;
    //Tooltip text for on hover when they've only clicked the building onces
    public string LevelText;
    //Tooltip text for on hover when they've already selected the building and need to tap again to confirm
    public string LevelText2;
    public bool Clicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
