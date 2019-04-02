using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, ShowVictoryText);
    }

    private void ShowVictoryText(GameEvent arg0)
    {
        this.gameObject.GetComponent<Renderer>().enabled = true;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
