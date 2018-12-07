using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Utility;

public class Broadcaster : Singleton<Broadcaster>
{
    [Serializable]
    public class BroadcastUnityEvent : UnityEvent<GameEvent> { }

    private readonly Dictionary<GameEvent, BroadcastUnityEvent> _subscribers = new Dictionary<GameEvent, BroadcastUnityEvent>();

    private void AddListen(GameEvent @event, UnityAction<GameEvent> action)
    {
        if (!_subscribers.ContainsKey(@event)) _subscribers.Add(@event, new BroadcastUnityEvent());
        _subscribers[@event].AddListener(action);
    }

    public static void AddListener(GameEvent @event, UnityAction<GameEvent> action)
    {
        Instance.AddListen(@event, action);
    }

    private void BroadcastState(GameEvent @event)
    {
        if (!_subscribers.ContainsKey(@event))
        {
            Debug.LogWarning($"No subscribers for @event {@event}");
            return;
        }
        if (Debugger.Profile.DebugGameState) Debug.Log($"Broadcast: {@event}");
        _subscribers[@event].Invoke(@event);
    }

    public static void Broadcast(GameEvent @event)
    {
        Instance.BroadcastState(@event);
    }
}
