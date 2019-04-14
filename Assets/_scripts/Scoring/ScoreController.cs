using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RideShareLevel;
using UnityEngine;

public enum BuildingScoreState { PlayerStar, EnemyStar, TBD };

/// <summary>
/// Manages all UI elements associated with the scoring for a level. Each level prefab
/// should have exactly one Score Controller - A Score Controller gameObject with this
/// script attatched as a component. This class should never be manually instantiated, however
/// Initialize() should be called upon loading a new level.
/// </summary>
public class ScoreController : LevelObject
{
    public BuildingScore BuildingScorePrefab;  // UI prefab to display above buildings, used in BuildingScore objects				

    #region Public Methods

    /// <summary>
    /// Links required Controllers to the ScoreController and initializes
    /// UI elements accordingly. Initialize() uses data from these controllers to
    /// construct the UI, so they MUST be initialized before passing them
    /// to the ScoreController's Initialize().
    /// </summary>
    public void Initialize()
    {
        if (!PassengerController.Initialized())
            throw new Exception("Score Controller was given an uninitialized Passenger Controller. " +
                            "Did you forget to call PassengerController.Initialize() before passing the controller" +
                            " to ScoreController.Initialize() ?");

        if (!EntityController.Initialized())
            throw new Exception("Score Controller was given an uninitialized Entity Controller. " +
                            "Did you forget to call EntityController.Initialize() before passing the controller" +
                            " to ScoreController.Initialize() ?");

        foreach (Building building in EntityController.Buildings)
        {
            building.InitializeScoreUI(BuildingScorePrefab);
        }

        Broadcaster.AddListener(GameEvent.BuildingComplete, BuildingComplete);
    }
    #endregion

    #region Event Handlers

    /// <summary>
    /// Updates the star rating UI for this building.
    /// Returns the new state of the building
    /// </summary>
    /// <param name="PC"> The PassengerController associated with this level </param>
    public BuildingScoreState GetStatusForBuilding(Building.BuildingColors color)
    {
        if (!PassengerController.GetSpawnStatus(color))
        {
            if (PassengerController.GetPlayerPassengersDelivered(color) > PassengerController.GetEnemyPassengersDelivered(color))
            {
                // Player won building
                return BuildingScoreState.PlayerStar;
            }
            else
            {
                // Enemy won building
                return BuildingScoreState.EnemyStar;
            }
        }

        return BuildingScoreState.TBD;
    }

    /// <summary>
    /// Handles the BuildingComplete Broadcast.
    /// Checks for GameOver and broadcasts accordingly
    /// </summary>
    private void BuildingComplete(GameEvent action)
    {
        bool gameOver = EntityController.Buildings.All(building => GetStatusForBuilding(building.BuildingColor) != BuildingScoreState.TBD);           // true only if NONE of the buildings are still TBD (active)
        bool success = EntityController.Buildings.Any(building => GetStatusForBuilding(building.BuildingColor) == BuildingScoreState.PlayerStar);			// true if the player has AT LEAST 1 star

        if (gameOver)
        {
            if (success) { Debug.Log("Success Broadcasted!"); Broadcaster.Broadcast(GameEvent.LevelCompleteSuccess); }
            else { Debug.Log("Fail Broadcasted!"); Broadcaster.Broadcast(GameEvent.LevelCompleteFail); }
        }
    }

    #endregion

}