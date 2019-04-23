using System;
using System.Collections;
using System.Collections.Generic;
using RideShareLevel;
using UnityEngine;
using UnityEngine.UI;

public class BuildingScore : LevelObject
{
    public CanvasGroup CanvasGroup;
    public Image StarIcon;
    public Image XIcon;
    public Text PlayerScoreText;
    public Text EnemyScoreText;

    public AudioSource BuildingFailSound;
    public AudioSource BuildingWinSound;

    private Camera _camera;
    private Building _building;
    #region Unity Methods

    private BuildingScoreState _state = BuildingScoreState.TBD;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (CanvasGroup.gameObject.activeInHierarchy)
        {
            CanvasGroup.transform.LookAt(_camera.transform, Vector3.up);
        }
    }

    #endregion

    public void SetBuilding(Building building)
    {
        _building = building;
        XIcon.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the scoring UI for this building
    /// </summary>
    /// <param name="PC"> The PassengerController associated with this level </param>
    public void UpdateDelivered()
    {
        var state = ScoreController.GetStatusForBuilding(_building.BuildingColor);
        if (state == BuildingScoreState.TBD)
        {
            PlayerScoreText.text = PassengerController.GetPlayerPassengersDelivered(_building.BuildingColor) + "/" + PassengerController.GetPassengersRequired(_building.BuildingColor);
            EnemyScoreText.text = PassengerController.GetEnemyPassengersDelivered(_building.BuildingColor) + "/" + PassengerController.GetPassengersRequired(_building.BuildingColor);
        }
    }

    /// <summary>
    /// Updates the star rating UI for this building.
    /// </summary>
    /// <param name="PC"> The PassengerController associated with this level </param>
    public void UpdateState()
    {
        var state = ScoreController.GetStatusForBuilding(_building.BuildingColor);
        if (_state != state)
        {
            switch (state)
            {
                case BuildingScoreState.PlayerStar:
                    StarIcon.color = Game.ColorKey.UIStarSuccess;
                    SetTextInactive();
                    BuildingWinSound.Play();
                    break;
                case BuildingScoreState.EnemyStar:
                    StarIcon.gameObject.SetActive(false);
                    XIcon.gameObject.SetActive(true);
                    SetTextInactive();
                    BuildingFailSound.Play();
                    break;
                case BuildingScoreState.TBD:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    private void SetTextInactive()
    {
        // set text to "inactive" to convey the building can no longer be delivered to
        PlayerScoreText.color = Game.ColorKey.UITextInactive;
								EnemyScoreText.color = Game.ColorKey.UITextInactive;
    }
}
