using RideShareLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages construction of the level and high level state
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    public Level CurrentLevel;

    #region Unity Methods

    private void Start()
    {

    }

    #endregion

    public void Initialize()
    {
    }

    public void GenerateLevel(Level data)
    {
        throw new NotImplementedException();
    }

    public void SaveLevel(Level data)
    {
        throw new NotImplementedException();
        // var json = JsonUtility.ToJson(data);
    }

    public void LoadLevel(Level data)
    {
        var level = JsonUtility.FromJson<Level>("{}");
        GenerateLevel(level);
    }

    public void TransitionLevel(MenuBuilding newLevel)
    {
        //Destroy old level object
        var oldLevel = CurrentLevel.gameObject;

        //Need to reset passenger manager
        ScoreManager.Instance.SetPassengerSpecs(newLevel.passengerSpecs);
        //Set game timer
        ScoreManager.Instance.GameTimer = newLevel.GameTimer;
        //Switch music to music of new level
        Osborne_AudioManager.Instance.SwitchLevels(newLevel.NewLayer1, newLevel.NewLayer2, newLevel.NewLayer3);

        newLevel.RepresentedLevel.GetComponent<Level>().PassengerSpecs = newLevel.passengerSpecs;

        //Spawn and place new level prefab in that spot.
        CurrentLevel = Instantiate(newLevel.RepresentedLevel, oldLevel.transform.position, oldLevel.transform.rotation).GetComponent<Level>();
        Destroy(oldLevel);
        CurrentLevel.EntityController.Initialize();

        //Notify controllers of the new gamestate
        Broadcaster.Broadcast(GameEvent.GameStateChanged);
    }

}
