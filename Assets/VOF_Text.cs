using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VOF_Text : MonoBehaviour
{
    //This class listens for victory or failure and sets the main text UI element of the GameEndCanvas to 
    //Succes or failure based on which event it recieves. 
    void Start()
    {
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, ChangeTextToSuccess);
        Broadcaster.AddListener(GameEvent.LevelCompleteFail, ChangeTextToFailure);
    }

    private void ChangeTextToFailure(GameEvent arg0)
    {
        this.gameObject.GetComponent<UnityEngine.UI.Text>().text = "Failure";
    }

    private void ChangeTextToSuccess(GameEvent arg0)
    {
        this.gameObject.GetComponent<UnityEngine.UI.Text>().text = "Success!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
