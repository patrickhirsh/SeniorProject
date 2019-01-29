using Level;
using System.Linq;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    #region Singleton
    private static PassengerManager _instance;
    public static PassengerManager Instance => _instance ?? (_instance = Create());

    private static PassengerManager Create()
    {
        GameObject singleton = FindObjectOfType<PassengerManager>()?.gameObject;
        if (singleton == null)
        {
            singleton = new GameObject { name = $"[{typeof(PassengerManager).Name}]" };
            singleton.AddComponent<PassengerManager>();
        }
        return singleton.GetComponent<PassengerManager>();
    }
    #endregion

    private Terminal[] _terminals;

    public Passenger PassengerPrefab;

    public static float PassengerTimeout = 60.0f;
    public float SpawnTime = 30.0f;
    public int PassengersToSpawn = 30;
    private int _passengerCount = 0;
    private float _timer;

    #region Unity Methods

    // Use this for initialization
    private void Start()
    {
        // Populate the Pickups list with every pickup in the scene
        _terminals = EntityManager.Instance.Routes.SelectMany(route => route.Terminals).ToArray();
        Debug.Assert(_terminals.Any(), "Missing terminals for the level. Has the EntityManager been baked?");
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.CurrentGameState == GameState.LevelSimulating)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                if (_passengerCount <= PassengersToSpawn)
                {
                    SpawnPassenger();
                    _passengerCount++;
                    _timer = SpawnTime;
                }            
            }
        }
    }

    #endregion

    private void SpawnPassenger()
    {
        int index = Random.Range(0, _terminals.Length - 1);

        // keep trying to spawn a passenger until we find an empty terminal
        while (!_terminals[index].SpawnPassenger(PassengerPrefab))
            index = Random.Range(0, _terminals.Length - 1); ;
    }
}