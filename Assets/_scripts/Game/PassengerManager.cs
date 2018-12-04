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

    public float SpawnTime = 5.0f;
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
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            SpawnPassenger();
            _timer = SpawnTime;
        }
    }

    #endregion

    private void SpawnPassenger()
    {
        int index = Random.Range(0, _terminals.Length - 1);
        _terminals[index].SpawnPassenger(PassengerPrefab);
    }
}