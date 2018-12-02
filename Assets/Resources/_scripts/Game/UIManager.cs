using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public GameObject Score;
    public int scoreValue;
	// Use this for initialization
	void Start ()
    {
        scoreValue = 0;
        Score.GetComponent<Text>().text = "Score: " + scoreValue;
	}

    /// <summary>
    /// This methods takes in a value update to the score and then updates the UI score
    /// </summary>
    /// <param name="scoreMod">Value to modify the current score</param>
    public void UpdateScore(int scoreMod)
    {
        scoreValue = scoreMod + scoreValue;
        Score.GetComponent<Text>().text = "Score: " + scoreValue;
    }

    public void TestScore()
    {
        UpdateScore(1000);
    }

    public void RefreshScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
	
	
}
