using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{

    public GameObject Children;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOn()
    {
        Children.SetActive(true);
    }

    public void TurnOff()
    {
        Children.SetActive(false);
    }

    public void HandleRestartButton()
    {
        Broadcaster.Broadcast(GameEvent.Reset);
        GameManager.SetGameState(GameState.LevelRePlacement);
        TurnOff();
    }
}
