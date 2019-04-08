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
    //Bool for firing fireworks
    private bool fireworks = false;
    //timer for firing fireworks
    private float fTimer;
    //interval at which to fire firewors
    public float FireworkInterval;
    //interval variance for firing firewors
    public float FireworkIntervalVariance;
    //variance of fireworks position
    public float FireworkPositionVariance;
    #region Unity Methods

    private void Start()
    {
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, LaunchSuccessFireworks);
    }

    private void FixedUpdate()
    {

        if (fireworks && (fTimer <= 0))
        {
            var randPos = CurrentLevel.transform.position;
            randPos.x += Random.Range(-FireworkPositionVariance, FireworkPositionVariance);
            randPos.y += Random.Range(-FireworkPositionVariance, FireworkPositionVariance);
            randPos.z += Random.Range(-FireworkPositionVariance, FireworkPositionVariance);
            ParticleManager.Instance.GenerateFirework(randPos);
            fTimer += FireworkInterval + Random.Range(-FireworkIntervalVariance, FireworkIntervalVariance);
        }
        fTimer -= .01f;
    }

    private void LaunchSuccessFireworks(GameEvent arg0)
    {
        fireworks = true;
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
        fireworks = false;
        //Destroy old level object
        var oldLevel = CurrentLevel.gameObject;

        //Set game timer
        ScoreManager.Instance.GameTimer = newLevel.GameTimer;
        //Switch music to music of new level
        Osborne_AudioManager.Instance.SwitchLevels(newLevel.NewLayer1, newLevel.NewLayer2, newLevel.NewLayer3);

        //Spawn and place new level prefab in that spot.
        CurrentLevel = Instantiate(newLevel.RepresentedLevel, oldLevel.transform.position, oldLevel.transform.rotation).GetComponent<Level>();
        Destroy(oldLevel);
        CurrentLevel.EntityController.Initialize();

        //Notify controllers of the new gamestate
        Broadcaster.Broadcast(GameEvent.GameStateChanged);
    }

}
