using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerScript : MonoBehaviour {

    private int score;
    private int minScoreVal = (int)(10 / PassengerManager.PassengerTimeout);
    private Dictionary<Building.BuildingColors, int> scoreDic;

    public List<PassengerTypes> passengerSpecs;
    public int oneStarNum;
    public int twoStarNum;
    public int threeStarNum;



    public enum CarType { LX, STD, VAN };

    // Use this for initialization
    void Awake() {
        scoreDic = new Dictionary<Building.BuildingColors, int>();
        foreach(PassengerTypes p in passengerSpecs)
        {
            scoreDic.Add(p.passColor, 0);
        }

    }



    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// This function should be called when passenger(s) are dropped off at a location to increment the score correctly. 
    /// </summary>
    /// <param name="timerLeft">This is how long the passenger had left on their timer when they were picked up/dropped off, whichever we decide is better</param>
    /// <param name="carType">This is the type of car the passenger was picked up in</param>
    /// <param name="numDelivered">This is the number of passengers dropped off at the location when this function was called</param>
    public void ScorePoints(Building.BuildingColors passengerColor, int numDelivered)
    {
        scoreDic[passengerColor] += numDelivered;
    }


    /// <summary>
    /// Function that returns the current score as a number of stars, indicating success level. 
    /// </summary>
    /// <returns></returns>
    public int GetStars()
    {
        if (score <= oneStarNum)
        {
            return 0;
        }
        else if (score < twoStarNum)
        {
            return 1;
        }
        else if (score < threeStarNum)
        {
            return 2;
        }
        else
        {
            return 3;
        }

    }

    public int GetCurrentScore(Building.BuildingColors color)
    {
        return scoreDic[color];
    }

    [System.Serializable]
    public class PassengerTypes
    {
        public int numSpawn;
        public int numRequired;
        public Building.BuildingColors passColor;
    }

    public List<Building.BuildingColors> GetBuildingColors()
    {
        return new List<Building.BuildingColors>(scoreDic.Keys);
    }
}
