using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputController : MonoBehaviour {

    //Whether a car has been selected to drag-control a path for
    private bool CarSelected;
    //The current selected car
    private VehicleEntity.Vehicle currentVehicle;
    //The inbbound connection last pathed to on the path
    private Level.Connection currentConnection;
    //The list of curves used to construct a curve path
    private List<BezierCurve> curves;

	// Use this for initialization
	void Start () {
        CarSelected = false;
        currentConnection = new Level.Connection();
        currentVehicle = new VehicleEntity.Vehicle();
        curves = new List<BezierCurve>();
	}

	// Update is called once per frame
	void Update () {
        //If you press dowwn and hit a car, select that car
        if (Input.GetMouseButtonDown(0))
        {
            //reset these values
            currentConnection = new Level.Connection();
            currentVehicle = new VehicleEntity.Vehicle();
            curves = new List<BezierCurve>();

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

            if (hit)
            {
                Debug.Log("Got Hit");
                if (hitInfo.transform.gameObject.GetComponent<VehicleEntity.Vehicle>())
                {
                    currentVehicle = hitInfo.transform.gameObject.GetComponent<VehicleEntity.Vehicle>();
                    CarSelected = true;
                    curves = new List<BezierCurve>();
                    Debug.Log("Hit Car");
                    //curves.Add(The path the car is currently on);
                }
            }
        }

        //get dragging movements
        if (Input.GetMouseButton(0) && CarSelected)
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            //if you hit an outbound node
            if (hit)
            {
                if (hitInfo.transform.gameObject.GetComponent<Level.Connection>() != null)
                {
                    if (hitInfo.transform.gameObject.GetComponent<Level.Connection>().Traveling == Level.Connection.TravelingDirection.Outbound)
                    {
                        Debug.Log("Hit an outbound node");
                        //then you can add the path from the previous inbound node to this outbound node to the path
                        BezierCurve newPath;
                        var connection = hitInfo.transform.gameObject.GetComponent<Level.Connection>();
                        currentConnection.FindPathToConnection(connection, out newPath);
                        curves.Add(newPath);

                        //and then set the "currentNode" to be the inbound node that connects to the outbound node hit
                        currentConnection = connection.ConnectsTo;
                        //Debug.Log("Hit node");
                    }
                }
            }
        }

        //when you stop touching, the curve is calcuated and the vehicle is set to travel the path you dragged
        if (Input.GetMouseButtonUp(0))
        {
            var curve = currentVehicle.transform.GetOrAddComponent<BezierCurve>();
            foreach (var point in curves.SelectMany(b => b.GetAnchorPoints()))
            {
                curve.AddPoint(point);
            }
            //Set car's new path to curves
            StartCoroutine(currentVehicle.TravelPath(curve));
        }

    }
}
