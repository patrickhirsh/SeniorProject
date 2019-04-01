using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerScript : MonoBehaviour {

    //Score integer (deprecated?)
    private int score;
    //Minimum score val to complete level (Deprecated?)
    private int minScoreVal = (int)(10 / PassengerManager.PassengerTimeout);
    //Dictionary of colored passenger requirments and their requirement numbers
    private Dictionary<Building.BuildingColors, int> scoreDic;
    
    //Designer specified passneger specifications
    public List<PassengerTypes> passengerSpecs;

    //Car type enumerations
    public enum CarType { LX, STD, VAN };

    // Use this for initialization
    void Awake()
    {
        SetDictionary();

    }

    //Set the scordDic dictionary with updated buildings
    private void SetDictionary()
    {
        scoreDic = new Dictionary<Building.BuildingColors, int>();
        foreach (PassengerTypes p in passengerSpecs)
        {
            scoreDic.Add(p.passColor, p.numRequired);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void VictoryCondition()
    {
        //Stop generating new passengers

        //Remove all unpicked up passengers from play or let them time out?

        //Generate final score (3 stars?) based on timer

        //Highlight building to use as return menu/just make the score into the return object?

        //Fire off fireworks for victory
        
        //throw new NotImplementedException();
    }

    private void FailureCondition()
    {
        //Stop generating new passengers

        //Remove all unpicked up passengers from play or let them time out?

        //Generate final score (3 stars?) based on timer

        //Highlight building to use as return menu/just make the score into the return object?

        //Fire off fireworks for victory

        //throw new NotImplementedException();

    }

    /// <summary>
    /// This function should be called when passenger(s) are dropped off at a location to increment the score correctly. 
    /// </summary>
    /// <param name="timerLeft">This is how long the passenger had left on their timer when they were picked up/dropped off, whichever we decide is better</param>
    /// <param name="carType">This is the type of car the passenger was picked up in</param>
    /// <param name="numDelivered">This is the number of passengers dropped off at the location when this function was called</param>
    public void ScorePoints(Building.BuildingColors passengerColor, int numDelivered)
    {
        scoreDic[passengerColor] -= numDelivered;
        if(scoreDic[passengerColor] == 0)
        {
            if (CheckGameEndState())
            {
                VictoryCondition();

            }
        }
    }

    private bool CheckGameEndState()
    {
        foreach (Building.BuildingColors pass in scoreDic.Keys)
        {
            if (scoreDic[pass] >= 0)
            {
                return false;
            }
        }
        return true;
    }

    public int GetCurrentScore(Building.BuildingColors color)
    {
        return scoreDic[color];
    }

    public List<Building.BuildingColors> GetBuildingColors()
    {
        return new List<Building.BuildingColors>(scoreDic.Keys);
    }

    internal int getNumRequired(Building.BuildingColors color)
    {
        return passengerSpecs.Find(x => x.passColor == color).numRequired;
    }

    internal void SetPassengerSpecs(List<PassengerTypes> newpassengerSpecs)
    {
        passengerSpecs = new List<PassengerTypes>();
        passengerSpecs = newpassengerSpecs;
        SetDictionary();
    }
}

[System.Serializable]
public class PassengerTypes
{
    public int numSpawn;
    public int numRequired;
    public Building.BuildingColors passColor;

}
