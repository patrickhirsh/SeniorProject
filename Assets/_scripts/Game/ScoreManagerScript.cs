using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerScript : MonoBehaviour {

    private int score;
    private int minScoreVal = (int)(10 / PassengerManager.PassengerTimeout);

    public int oneStarNum;
    public int twoStarNum;
    public int threeStarNum;


    public enum CarType { LX, STD, VAN};

	// Use this for initialization
	void Start () {
        score = 0;
	}


	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// This function should be called when passenger(s) are dropped off at a location to increment the score correctly. 
    /// </summary>
    /// <param name="timerLeft">This is how long the passenger had left on their timer when they were picked up/dropped off, whichever we decide is better</param>
    /// <param name="carType">This is the type of car the passenger was picked up in</param>
    /// <param name="numDelivered">This is the number of passengers dropped off at the location when this function was called</param>
    public void ScorePoints(float timerLeft, CarType carType, int numDelivered)
    {
        float retval = 100;

        retval = retval * ((timerLeft/PassengerManager.PassengerTimeout)*2 + minScoreVal);
        switch (carType)
        {
            case CarType.LX:
                retval *= 10;
                break;
            case CarType.STD:
                retval *= 5;
                break;
            case CarType.VAN:
                retval *= 3;
                break;
            default:
                break;
        }

        retval *= numDelivered;
        score += Mathf.CeilToInt(retval);

    }


    /// <summary>
    /// Function that returns the current score as a number of stars, indicating success level. 
    /// </summary>
    /// <returns></returns>
    public int GetStars()
    {
        if(score <= oneStarNum)
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

    public int GetCurrentScore()
    {
        return score;
    }
}
