using Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VehicleEntity;

public class InputManager : MonoBehaviour
{
    #region Singleton
    private static InputManager _instance;
    public static InputManager Instance => _instance ?? (_instance = Create());

    private static InputManager Create()
    {
        var singleton = FindObjectOfType<InputManager>()?.gameObject;
        if (singleton == null) singleton = new GameObject { name = typeof(InputManager).Name };
        singleton.AddComponent<InputManager>();
        return singleton.GetComponent<InputManager>();
    }
    #endregion

    //The inbound connection last pathed to on the path. MUST ALWAYS BE AN INBOUND CONNECTION
    public GameObject NextIndicatorPrefab;
    public GameObject CurrentIndicatorPrefab;

    private Connection _currentConnection;
    private bool HasCurrentConnection => _currentConnection != null;

    //The current selected car
    private Vehicle _currentVehicle;
    private bool CarSelected => _currentVehicle != null;

    //The list of curves used to construct a curve path
    private List<BezierCurve> _curves;
    private List<GameObject> _indicators;
    private GameObject _currentIndicator;

    // Use this for initialization
    private void Start()
    {
        _curves = new List<BezierCurve>();
        _indicators = new List<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        //If you press down and hit a car, select that car
        if (Input.GetMouseButtonDown(0))
        {
            //reset these values
            _curves = new List<BezierCurve>();

            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                var target = hitInfo.transform.GetComponent<Vehicle>();
                if (target != null)
                {
                    _currentVehicle = target;
                    _currentConnection = _currentVehicle.ClosestInbound;
                    _currentVehicle.StopTraveling();
                    //curves.Add(The path the car is currently on);
                }
            }
        }   

        //get dragging movements
        if (Input.GetMouseButton(0) && CarSelected && HasCurrentConnection)
        {
            DestroyIndicators();
            DrawIndicators();

            RaycastHit hitInfo;
            //if you hit an outbound node
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                var target = hitInfo.transform.GetComponent<Connection>();
                if (target != null && target.OutboundOrInternal)
                {
                    BezierCurve curve;
                    if (_currentConnection.GetPathToConnection(target, out curve))
                    {
                        //then you can add the path from the previous inbound node to this outbound node to the path
                        _curves.Add(curve);

                        //and then set the "currentNode" to be the inbound node that connects to the outbound node hit
                        _currentConnection = target.ConnectsTo;
                    }
                }
            }
        }

        //when you stop touching, the curve is calculated and the vehicle is set to travel the path you dragged
        if (Input.GetMouseButtonUp(0) && _curves.Any())
        {
            _currentVehicle.TravelPath(_curves);
            DestroyIndicators();
        }
    }

    private void DestroyIndicators()
    {
        Destroy(_currentIndicator);
        _indicators.ForEach(Destroy);
    }

    private void DrawIndicators()
    {
        _currentIndicator = Instantiate(CurrentIndicatorPrefab, _currentConnection.transform, false);

        // TODO: Object Pooling
        foreach (var connection in _currentConnection.ConnectedConnections())
        {
            _indicators.Add(Instantiate(NextIndicatorPrefab, connection.transform, false));
        }
    }
}