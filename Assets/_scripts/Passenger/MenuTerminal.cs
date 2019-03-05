using Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTerminal : MonoBehaviour
{
    //Private Class Variables
    private Passenger Passenger;
    //Public Class Variabbles
    public Route ParentRoute => Connection.ParentRoute;
    public Connection Connection;
    public GameObject Passengerprefab;
    public Building Destination;
    // Start is called before the first frame update
    void Start()
    {
        Passenger = Instantiate(Passengerprefab, ParentRoute.CenterTransform, false).GetComponent<Passenger>();
        Passenger.GetComponent<Pin>().GetComponent<Renderer>().material = Destination.GetComponent<Renderer>().material;
        Passenger.DestRoute = Destination.DeliveryLocation;
        Passenger.LevelPrefab = Destination.gameObject.GetComponent<Menubuilding>().LevelPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
