using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextScript : MonoBehaviour {
    //Reference to score manager
    ScoreManager SM;
    /// <summary>
    /// Icon prefab for insatiating icons
    /// </summary>
    public GameObject iconPrefab;
    /// <summary>
    /// Dictionary of all scoring icons, key is the color. 
    /// </summary>
    private Dictionary<Building.BuildingColors, GameObject> ScoreIcons;
    /// <summary>
    /// Set of icon materials. 
    /// </summary>
    public Material RedMat;
    public Material BlueMat;
    public Material GreenMat;
    public Material YellowMat;
    public Material PurpleMat;
    public Material OrangeMat;


    // Use this for initialization
    void Awake () {
        SM = GameObject.FindObjectOfType<ScoreManager>();
        ScoreIcons = new Dictionary<Building.BuildingColors, GameObject>();
        
	}

    void Start()
    {
        CreateScoreIcons();
    }

    /// <summary>
    /// Gets material object that should be assigned to icon from the color enum provided. 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
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



    // Update is called once per frame
    void Update ()
    {
        UpdateScore();
    }

    // <summary>
    // Updates the scoretext object in the scene.
    // I extracted this so that we can do a explicit update if we want, but it shouldn't be neccessary as long as the score 
    // manager continues to update it's value. 
    //</summary>
    private void UpdateScore()
    {
        int TempScore = 0;
        foreach (KeyValuePair<Building.BuildingColors, GameObject> kvp in ScoreIcons) {
            TempScore = SM.GetCurrentScore(kvp.Key);
            if (kvp.Value.GetComponent<ScoreIcon>().Score != TempScore)
            {
                ScoreIcons[kvp.Key].GetComponent<ScoreIcon>().Score = TempScore;
                //This is a spot we could initiate some kind of cool effect for ticking up the score, I just don't know how to do that
                ScoreIcons[kvp.Key].GetComponentInChildren<TextMesh>().text = TempScore.ToString();

            }
        }
    }

    /// <summary>
    /// Public function to reset score object in scene
    /// </summary>
    public void ResetScore()
    {
        ScoreIcons.Clear();
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
        foreach (Building.BuildingColors color in SM.GetBuildingColors())
        {
            ScoreIcons.Add(color, CreateIcon(color, xAdjustment));
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

        GameObject NewIcon = Instantiate(iconPrefab, this.transform.position + new Vector3(adjustmentInt, 0, 0), this.transform.rotation, this.transform);
        NewIcon.GetComponentInChildren<IconImage>().ChangeColr(GetMatFromColorName(color));
        NewIcon.GetComponentInChildren<TextMesh>().text = "Balls";
        int initialScoreValue = SM.getNumRequired(color);

        NewIcon.GetComponent<ScoreIcon>().Score = initialScoreValue;
        NewIcon.GetComponentInChildren<TextMesh>().text = initialScoreValue.ToString();
        return NewIcon;
    }
}
