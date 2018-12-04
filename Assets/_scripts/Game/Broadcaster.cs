using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Utility;

public class Broadcaster : MonoBehaviour
{
    #region Singleton
    private static Broadcaster _instance;
    public static Broadcaster Instance => _instance ?? (_instance = Create());

    private static Broadcaster Create()
    {
        GameObject singleton = FindObjectOfType<Broadcaster>()?.gameObject;
        if (singleton == null) singleton = new GameObject { name = typeof(Broadcaster).Name };
        if (singleton.GetComponent<Broadcaster>() == null) singleton.AddComponent<Broadcaster>();
        return singleton.GetComponent<Broadcaster>();
    }

    #endregion

    [Serializable]
    public class BroadcastUnityEvent : UnityEvent<GameState> { }

    private readonly Dictionary<GameState, BroadcastUnityEvent> _subscribers = new Dictionary<GameState, BroadcastUnityEvent>();

    public void AddListener(GameState state, UnityAction<GameState> action)
    {
        if (!_subscribers.ContainsKey(state)) _subscribers.Add(state, new BroadcastUnityEvent());
        _subscribers[state].AddListener(action);
    }

    public void Broadcast(GameState state)
    {
        if (!_subscribers.ContainsKey(state))
        {
            Debug.LogWarning($"No subscribers for state {state}");
            return;
        }
        if (Debugger.Profile.DebugGameState) Debug.Log($"Broadcast: {state}");
        _subscribers[state].Invoke(state);
    }
}
