using UnityEngine;

public class SlowDownBehavior : MonoBehaviour
{
    // How far to cast in front of this object
    [SerializeField]
    private readonly float _castDistance = 1.0f;

    // The front of the vehicle
    [SerializeField]
    private Transform _front;

    private float _runningVelocity = 1.0f;

    private float _velocity = 1.0f;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        // Cast a ray in front of this object
        RaycastHit hit;
        Physics.Raycast(_front.position, _front.forward, out hit, _castDistance);

        // If there was a hit
        if (hit.point != Vector3.zero)
        {
            var dist = Vector3.Distance(_front.position, hit.point);

            var unitDistance = dist / _castDistance;

            _velocity = Mathf.Lerp(_velocity, 0, 1 - unitDistance);
        }
    }
}