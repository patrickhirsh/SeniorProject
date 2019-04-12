using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RideShareLevel;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialState { SelectPassenger, SelectCar, DeliveringPassenger, ScoreDisplay, PassengerDisplay }

    public GameObject TutorialText;

    public GameObject ScoreText;

    [SerializeField]
    private List<Vehicle> PlayerVehicles;

    private TutorialState state = TutorialState.SelectPassenger;

    private GameObject SelectedVehicle;

    private GameObject ScoreDisplayText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (state == TutorialState.SelectPassenger)
        {
            var passengerObjs = FindObjectsOfType<Passenger>();

            foreach (var passenger in passengerObjs)
            {
                if (passenger.GetComponentInChildren<TextMesh>() == null)
                {
                    var textObject = GameObject.Instantiate(TutorialText, passenger.transform.position, Quaternion.identity);
                    textObject.GetComponent<TextMesh>().text = "Select a passenger";

                    textObject.transform.parent = passenger.gameObject.transform;
                }
            }
        }
        else if (state == TutorialState.SelectCar)
        {
            foreach (var vehicle in PlayerVehicles)
            {
                if (vehicle.GetComponentInChildren<TextMesh>() == null)
                {
                    var textObject = GameObject.Instantiate(TutorialText, vehicle.transform.position, Quaternion.identity);
                    textObject.GetComponent<TextMesh>().text = "Select a car";

                    textObject.transform.parent = vehicle.gameObject.transform;
                }
            }
        }
        else if (state == TutorialState.DeliveringPassenger)
        {
        }
        else if (state == TutorialState.ScoreDisplay)
        {

        }
        else if (state == TutorialState.PassengerDisplay)
        {
        }
    }

    public void PickupPassenger()
    {

        if (state == TutorialState.SelectPassenger)
        {

            var passengerObjs = FindObjectsOfType<Passenger>();

            foreach (var passenger in passengerObjs)
            {
                if (passenger.GetComponentInChildren<TextMesh>() != null)
                {
                    Destroy(passenger.GetComponentInChildren<TextMesh>().gameObject);
                }
            }

            state = TutorialState.SelectCar;
        }
    }

    public void SelectVehicle(Vehicle vehicle)
    {

        if (state == TutorialState.SelectCar)
        {
            foreach (var v in PlayerVehicles)
            {
                if (v.GetComponentInChildren<TextMesh>() != null)
                {
                    Destroy(v.GetComponentInChildren<TextMesh>().gameObject);
                }
            }

            var textObject = GameObject.Instantiate(TutorialText, vehicle.transform.position, Quaternion.identity);
            textObject.GetComponent<TextMesh>().text = "Delivering Passenger";

            textObject.transform.parent = vehicle.gameObject.transform;

            SelectedVehicle = vehicle.gameObject;

            state = TutorialState.DeliveringPassenger;
        }
    }

    public void DeliverPassenger()
    {
        if (state == TutorialState.DeliveringPassenger)
        {
            Destroy(SelectedVehicle.GetComponentInChildren<TextMesh>().gameObject);

            ScoreDisplayText = GameObject.Instantiate(TutorialText, ScoreText.transform.position, Quaternion.identity);
            ScoreDisplayText.GetComponent<TextMesh>().text = "Deliver this many more passengers before your opponent to receive a star!";
            ScoreDisplayText.transform.position += Vector3.up * 3.5f;

            state = TutorialState.ScoreDisplay;
        }
    }
}
