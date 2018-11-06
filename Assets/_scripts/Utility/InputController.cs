using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputController : MonoBehaviour {

    //Whether a car has been selected to drag-control a path for
    private bool CarSelected;
    //The current selected car
    private VehicleEntity.Vehicle currentVehicle;
    //The inbbound connection last pathed to on the path. MUST ALWAYS BE AN INBOUND CONNECTION
    public Level.Connection currentConnection;
    //The list of curves used to construct a curve path
    private List<BezierCurve> curves;

    public GameObject navPrefab;

    public bool indicator;

	// Use this for initialization
	void Start () {
        CarSelected = false;
   
        currentVehicle = new VehicleEntity.Vehicle();
        //currentConnection = new Level.Connection();

        curves = new List<BezierCurve>();
	}

	// Update is called once per frame
	void Update () {
        //If you press dowwn and hit a car, select that car
        if (Input.GetMouseButtonDown(0))
        {
            //reset these values
            
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
                    
                    //currentConnection = currentVehicle.GetComponent<Level.Connection>();
                    CarSelected = true;
                   
                    Debug.Log("Hit Car");
                    //curves.Add(The path the car is currently on);
                }
            }
        }

        //get dragging movements
        if (Input.GetMouseButton(0) && CarSelected)
        {
            //primitave way to indicate selectable paths
            if (indicator)
            {
                foreach (Level.Connection.ConnectionPath x in currentConnection.Paths)
                {

                    Instantiate(navPrefab, currentConnection.transform);
                }
            }
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            //if you hit an outbound node
            if (hit)
            {
                //If the node is a connection
                if (hitInfo.transform.gameObject.GetComponent<Level.Connection>() != null)
                {
                    //If the connection you hit is an outbbound
                    if (hitInfo.transform.gameObject.GetComponent<Level.Connection>().Type == Level.Connection.ConnectionType.Outbound)
                    {
                        Debug.Log("hit a connection");
                        //If the outbound connection you hit is directly connected to the inbound connection you started with
                        //if there is a better way to do this I'm open to it
                        foreach(Level.Connection.ConnectionPath x in currentConnection.Paths)
                        {
                            
                            Debug.Log(x.OutboundConnection.name);
                            Debug.Log(hitInfo.transform.gameObject.GetComponent<Level.Connection>().Equals(x.OutboundConnection));
                            if (x.OutboundConnection == hitInfo.transform.gameObject.GetComponent<Level.Connection>())
                            {
                                Debug.Log("Hit an outbound node");
                                //then you can add the path from the previous inbound node to this outbound node to the path
                                BezierCurve newPath;
                                var connection = hitInfo.transform.gameObject.GetComponent<Level.Connection>();
                                currentConnection.GetPathToConnection(connection, out newPath);
                                curves.Add(newPath);

                                //and then set the "currentNode" to be the inbound node that connects to the outbound node hit
                                currentConnection = connection.ConnectsTo;
                                //Debug.Log("Hit node");
                                break;
                            }
                        }

                    }
                }
            }
        }

        //when you stop touching, the curve is calcuated and the vehicle is set to travel the path you dragged
        if (Input.GetMouseButtonUp(0) && curves.Count > 0)
        {
            var curve = new BezierCurve();
            //curve = currentVehicle.transform.GetOrAddComponent<BezierCurve>();
            foreach (var point in curves.SelectMany(b => b.GetAnchorPoints()))
            {
                Debug.Log("Adding a curve");
                curve.AddPoint(point);
            }
            
            //Set car's new path to curves
            StartCoroutine(currentVehicle.TravelPath(curve));
        }

    }
}
