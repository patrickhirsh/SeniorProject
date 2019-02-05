using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextScript : MonoBehaviour {

    ScoreManagerScript SM;

    private int CurrentScore;
    private int TempScore;
	// Use this for initialization
	void Start () {
        SM = GameObject.FindObjectOfType<ScoreManagerScript>();
        CurrentScore = 0;
        TempScore = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateScore();

    }

    /// <summary>
    /// Updates the scoretext object in the scene. 
    /// I extracted this so that we can do a explicit update if we want, but it shouldn't be neccessary as long as the score 
    /// manager continues to update it's value. 
    /// </summary>
    private void UpdateScore()
    {
        TempScore = SM.GetCurrentScore();
        if (CurrentScore != TempScore)
        {
            CurrentScore = TempScore;
            //This is a spot we could initiate some kind of cool effect for ticking up the score, I just don't know how to do that
            this.gameObject.GetComponent<TextMesh>().text = CurrentScore.ToString();

        }
    }

    /// <summary>
    /// Public function to reset score object in scene
    /// </summary>
    public void ResetScore()
    {
        CurrentScore = 0;
        TempScore = 0;
        this.gameObject.GetComponent<TextMesh>().text = CurrentScore.ToString();
    }


}
