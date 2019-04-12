using System;
using System.Collections;
using System.Collections.Generic;
using RideShareLevel;
using UnityEngine;

public class ScoreTextScript : LevelObject
{
    /// <summary>
    /// Icon prefab for insatiating icons
    /// </summary>
    

    /// <summary>
    /// Dictionary of all scoring icons, key is the color. 
    /// </summary>
    private Dictionary<Building.BuildingColors, GameObject> _scoreIcons;

    /// <summary>
    /// Set of icon materials. 
    /// </summary>
    public Material RedMat;
    public Material BlueMat;
    public Material GreenMat;
    public Material YellowMat;
    public Material PurpleMat;
    public Material OrangeMat;

    #region Unity Methods

    // Use this for initialization
    void Awake()
    {
        _scoreIcons = new Dictionary<Building.BuildingColors, GameObject>();
        Broadcaster.AddListener(GameEvent.PassengerDelivered, UpdateScore);
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, ShutOffOnVictory);
    }


    void Start()
    {
        CreateScoreIcons();
    }

    #endregion

    /// <summary>
    /// Gets material object corresponding to the provided BuildingColor enum val
    /// </summary>
    private Material GetMatFromColorName(Building.BuildingColors color)
    {
        switch (color)
        {
            case Building.BuildingColors.Red:
                return RedMat;
            case Building.BuildingColors.Blue:
                return BlueMat;
            case Building.BuildingColors.Green:
                return GreenMat;
            case Building.BuildingColors.Yellow:
                return YellowMat;
            case Building.BuildingColors.Purple:
                return PurpleMat;
            case Building.BuildingColors.Orange:
                return OrangeMat;
            default:
                return RedMat;
        }
    }

    private void UpdateScore(GameEvent @event)
    {
        foreach (KeyValuePair<Building.BuildingColors, GameObject> kvp in _scoreIcons)
        {
            var passengersNeeded = CurrentLevel.PassengerController.GetPlayerPassengersDelivered(kvp.Key);
            if (kvp.Value.GetComponent<ScoreIcon>().Score != passengersNeeded)
            {
                _scoreIcons[kvp.Key].GetComponent<ScoreIcon>().Score = passengersNeeded;
                //This is a spot we could initiate some kind of cool effect for ticking up the score, I just don't know how to do that
                _scoreIcons[kvp.Key].GetComponentInChildren<TextMesh>().text = passengersNeeded.ToString();

            }
        }
    }

    /// <summary>
    /// Public function to reset score object in scene
    /// </summary>
    public void ResetScore()
    {
        _scoreIcons.Clear();
        CreateScoreIcons();
    }

    /// <summary>
    /// Dynamically instatiate the score icons, placing them in correct positions. Could be more accurate for even numbers of icons, but I'll work on that later. 
    /// </summary>
    /// //TODO: Make more accurate placement of score icons
    private void CreateScoreIcons()
    {
        int xAdjustment = 0;
        bool direction = true;
        foreach (Building.BuildingColors color in CurrentLevel.PassengerController.GetBuildingColors())
        {
            _scoreIcons.Add(color, CreateIcon(color, xAdjustment));
            if (direction)
            {
                xAdjustment += 5;
                xAdjustment *= -1;
                direction = false;
            }
            else
            {
                xAdjustment *= -1;
                direction = true;
            }
        }
    }

    /// <summary>
    /// Actual instatitations of icon object
    /// </summary>
    /// <param name="color"></param>
    /// <param name="adjustmentInt"></param>
    /// <returns></returns>
    private GameObject CreateIcon(Building.BuildingColors color, int adjustmentInt)
    {
								/*
        GameObject NewIcon = Instantiate(iconPrefab, this.transform.position + new Vector3(adjustmentInt, 0, 0), this.transform.rotation, this.transform);
        NewIcon.GetComponentInChildren<IconImage>().ChangeColr(GetMatFromColorName(color));
        NewIcon.GetComponentInChildren<TextMesh>().text = "Balls";
        int initialScoreValue = CurrentLevel.PassengerController.GetPassengersRequired(color);

        NewIcon.GetComponent<ScoreIcon>().Score = initialScoreValue;
        NewIcon.GetComponentInChildren<TextMesh>().text = initialScoreValue.ToString();
        return NewIcon;
								*/
    }


    private void ShutOffOnVictory(GameEvent arg0)
    {
        this.GetComponent<Renderer>().enabled = false;
    }
}
