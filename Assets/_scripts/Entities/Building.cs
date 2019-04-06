using Game;
using UnityEngine;

public class Building : MonoBehaviour
{
    /// <summary>
    /// Location that vehicles will deliver passengers to
    /// </summary>
    public RideShareLevel.Route DeliveryLocation;

    public enum BuildingColors { Red, Green, Blue, Yellow, Purple, Orange, BROKENDONOTSELECT }

    public BuildingColors BuildingColor;

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(DeliveryLocation != null, $"Delivery location is not set for {gameObject}", gameObject);
        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            rend.material.color = ColorKey.GetColor(BuildingColor);
        }
    }
}
