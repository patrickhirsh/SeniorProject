using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    private bool on;
    public GameObject Children;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCanvas()
    {
        if (on)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }

    public void TurnOn()
    {
        Children.SetActive(true);
        on = true;
    }

    public void TurnOff()
    {
        Children.SetActive(false);
        on = false;
    }

    public void HandleRestartButton()
    {
        Broadcaster.Broadcast(GameEvent.Reset);
        GameManager.SetGameState(GameState.LevelRePlacement);
        TurnOff();
    }

}
