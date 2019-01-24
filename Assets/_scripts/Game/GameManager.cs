using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the state of the game like transition between levels or menus
/// </summary>
public class GameManager : Singleton<GameManager>
{

    public Canvas UI;
    
    #region Unity Methods

    private void Start()
    {
        LevelManager.Instance.Initialize();
        SetGameState(GameState.LevelPlacement);
        Broadcaster.AddListener(GameEvent.Reset, Reset);
    }

    #endregion

    private void Reset(GameEvent @event)
    {
        _gameStates.Clear();
    }

    #region GameState

    private static readonly Stack<GameState> _gameStates = new Stack<GameState>();
    public static GameState? CurrentGameState => _gameStates.Any() ? _gameStates.Peek() : (GameState?)null;
    public static GameState? PreviousGameState => _gameStates.Any() ? _gameStates.ToArray()[_gameStates.Count - 2] : (GameState?)null;

    public static void SetGameState(GameState state)
    {
        if (CurrentGameState == state) return;
        Debug.Log($"GameState change from {CurrentGameState} to {state}");
        _gameStates.Push(state);
        Broadcaster.Broadcast(GameEvent.GameStateChanged);
    }

    public static void PopGameState()
    {
        var current = _gameStates.Pop();
        Debug.Log($"GameState change from {current} to {CurrentGameState}");
        Broadcaster.Broadcast(GameEvent.GameStateChanged);
    }

    #endregion


    //All of this should be done by score manager now
    //[Serializable]
    //public class ScoreEvent : UnityEvent<int> { }

    //public ScoreEvent ScoreChanged;

    //public int CurrentScore { get; private set; }

    //public void AddScore(int amount)
    //{
    //    SetScore(CurrentScore + amount);
    //}

    //private void SetScore(int i)
    //{
    //    CurrentScore = i;
    //    ScoreChanged?.Invoke(CurrentScore);
    //}

    public void gameOver()
    {
        UI.GetComponent<UserInterface.ScoreCanvas>().gameObject.SetActive(false);
        UI.GetComponent<GameOverScript>().gameObject.SetActive(true); 
    }

}
