using Game;
using RideShareLevel;
using UnityEngine;

public class Building : LevelObject
{
    [Header("Score UI")]
    private BuildingScore _buildingScore;
    public Transform ScoreLocation;

    /// Location that vehicles will deliver passengers to
    public Route DeliveryLocation;

    public enum BuildingColors
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        Orange,
        Pink,
        DISABLED
    }

    public BuildingColors BuildingColor;

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(DeliveryLocation != null, $"Delivery location is not set for {gameObject}", gameObject);
        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            rend.material.color = ColorKey.GetBuildingColor(BuildingColor);
        }
    }

    #region Score UI

    public void InitializeScoreUI(BuildingScore buildingScorePrefab)
    {
        _buildingScore = Instantiate(buildingScorePrefab, transform, true);
        _buildingScore.SetBuilding(this);
        _buildingScore.transform.position = FindScoreLocation();
        _buildingScore.UpdateDelivered();

        Broadcaster.AddListener(GameEvent.PassengerDelivered, PassengerDelivered);
        Broadcaster.AddListener(GameEvent.BuildingComplete, BuildingComplete);
    }

    private Vector3 FindScoreLocation()
    {
        var hit = Physics.Raycast(transform.position + Vector3.up * 5000, Vector3.down, out var hitInfo);
        return hitInfo.point + Vector3.up * 20;
    }

    private void BuildingComplete(GameEvent arg0)
    {
        _buildingScore.UpdateState();
    }

    public void PassengerDelivered(GameEvent arg0)
    {
        _buildingScore.UpdateDelivered();
    }

    #endregion


}
