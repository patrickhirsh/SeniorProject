using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool _carSelected; // indicates a car is currently selected. Waiting for a dest. click for pathing...
    private Car _currentCar; // when CarSelected == true, this is the currently selected car
    public bool InputDebugMode;


    // Use this for initialization
    private void Awake()
    {
        InputDebugMode = true;
        _carSelected = false; // reset CarSelected state       
    }


    // Update is called once per frame
    private void Update()
    {
        HandleInput();
    }


    private void HandleInput()
    {
        // Handle MouseDown
        if (Input.GetMouseButtonDown(0))
        {
            // determine click location
            var hitInfo = new RaycastHit();
            var hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

            // if the click was valid...
            if (hit)
            {
                // determine the object we've selected and act accordingly
                if (InputDebugMode) Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                switch (hitInfo.transform.gameObject.tag)
                {
                    case "Car":
                        _currentCar = hitInfo.transform.gameObject.GetComponent<Car>();
                        _carSelected = true;
                        break;

                    case "AngryCar":
                        break;

                    case "Node":
                        if (_carSelected)
                        {
                            if (InputDebugMode) Debug.Log("Pathing to Node");
                            var newNode = _currentCar.GetNextNode();
                            _currentCar.SetPath(newNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<NodeOld>()));
                            _carSelected = false;
                        }

                        break;

                    case "Parking Spot":
                        if (_carSelected)
                        {
                            if (InputDebugMode) Debug.Log("Pathing to ParkingSpotNode");
                            var newNode = _currentCar.GetNextNode();
                            _currentCar.SetPath(newNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<NodeOld>()));
                            _carSelected = false;
                        }

                        break;

                    default:
                        if (InputDebugMode) Debug.Log("No hit");
                        _carSelected = false;
                        break;
                }
            }

            else
            {
                Debug.Log("No hit");
                _carSelected = false;
            }
        }
    }
}