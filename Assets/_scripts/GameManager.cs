using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameObject Target; // need to swap all references to Control.Target with this one
    public static AngryCar ACar;
    public static ParkingSpotNodeOld StartNodeOld;

    // Use this for initialization
    private void Start()
    {
        // obtain a reference to AngryCar
        ACar = FindObjectOfType<AngryCar>();
        Target = GameObject.FindGameObjectWithTag("Target");
        StartNodeOld = GameObject.FindGameObjectWithTag("Start Node").GetComponent<ParkingSpotNodeOld>();
        // initialize static structures for non-singleton classes
        Car.Initialize();
        NodeOld.Initialize();
        ParkingSpotNodeOld.Initialize(4);
        Car.Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public static NodeOld GetStartNode()
    {
        return StartNodeOld;
    }
}