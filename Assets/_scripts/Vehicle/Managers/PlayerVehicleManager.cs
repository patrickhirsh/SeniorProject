using UnityEngine;
using System.Collections;
using Level;
using System;

public class PlayerVehicleManager : VehicleManager
{

    private bool vehicleSelected;
    private Level.Vehicle selectedVehicle;

    public void Awake()
    {
        vehicleSelected = false;
    }


    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {
        
    }

    internal void TakeInput(RaycastHit hitInfo)
    {
        if(hitInfo.transform.gameObject.GetComponent<Level.Vehicle>()){
            carSelection(hitInfo.transform.gameObject.GetComponent<Level.Vehicle>());
        }
        else if(hitInfo.transform.gameObject.GetComponent<Intersection>()){
            intersectionSelection(hitInfo.transform.gameObject.GetComponent<Intersection>());
        }
        else if(hitInfo.transform.gameObject.GetComponent<PickupLocation>()){
            pickupSelction(hitInfo.transform.gameObject.GetComponent<PickupLocation>());
        }
        
    }
    //process a tap on a pickup location
    private void pickupSelction(PickupLocation pickupLocation)
    {
        if(vehicleSelected){
            //
            
        }
        else{
            //No vehicle selected, so you can't navigate to the pickup 
        }
    }
    //process a tap on an intersection
    private void intersectionSelection(Intersection intersection)
    {
        if(vehicleSelected){
            //Add the task of navigating to this intersection to the que of things to do
            
        }
        else{
            //No vehicle selected, so you can't navigate to the intersection
        }
    }
    //process a tap on a car
    private void carSelection(Vehicle vehicle)
    {
        if(!vehicleSelected){
            //Pick a new vehicle to control
            selectedVehicle = vehicle;
            vehicleSelected = true;

        }
        else{
            // Deselect current car
            
        }
    }
}
