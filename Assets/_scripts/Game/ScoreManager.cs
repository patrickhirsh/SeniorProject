using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    /// <summary>
    ///Time limit on a given level. 
    /// </summary>
    public float GameTimer;
    //Car type enumerations
    public enum CarType { LX, STD, VAN };

    /// <summary>
    /// Function to call on Victory Condition Reached
    /// </summary>
    private void VictoryCondition()
    {
        //Stop generating new passengers
            

        //Remove all unpicked up passengers from play or let them time out?

        //Generate final score (3 stars?) based on timer


        //Highlight building to use as return menu/just make the score into the return object?

        //Fire off fireworks for victory

    }
    /// <summary>
    /// Function to call on failure condition (time out without enough delivered passengers)
    /// </summary>
    private void FailureCondition()
    {
        //Stop generating new passengers

        //Remove all unpicked up passengers from play or let them time out?

        //Generate final score (3 stars?) based on timer

        //Highlight building to use as return menu/just make the score into the return object?


    }
}

[System.Serializable]
public class PassengerTypes
{
    public int NumSpawn;
    public int NumRequired;
    public float InitialDelay;
    public int TimeBetweenSpawn;
    public int PassengerTimer;
    public Building.BuildingColors PassColor;
}
