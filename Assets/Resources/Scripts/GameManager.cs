using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameObject Target;   // need to swap all references to Control.Target with this one
    public static AngryCar ACar;
    public static ParkingSpotNode StartNode;
    public string level;

	// Use this for initialization
	void Start ()
    {
        // obtain a reference to AngryCar
        ACar = FindObjectOfType<AngryCar>();
        Target = GameObject.FindGameObjectWithTag("Target");
        StartNode = GameObject.FindGameObjectWithTag("Start Node").GetComponent<ParkingSpotNode>();
        // initialize static structures for non-singleton classes
        Car.Initialize();
        Node.Initialize();
        ParkingSpotNode.Initialize(4);
        AngryCar.Initialize();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public static Node GetStartNode()
    {
        return StartNode;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(level, LoadSceneMode.Single);

    }

    public void Quit()
    {
        Application.Quit();
    }
    /// <summary>
    /// Triggered by resume button and resumes the timeline of the game
    /// </summary>
    public void Resume()
    {

    }
    /// <summary>
    /// Sets the speed of the game to normal
    /// </summary>
    public void NormalSpeed()
    {

    }
    /// <summary>
    /// Sets the speed of the game to faster
    /// </summary>
    public void FastSpeed()
    {

    }
}
