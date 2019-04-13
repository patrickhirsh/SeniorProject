using System;
using System.Collections;
using System.Collections.Generic;
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
				public GameObject BuildingScorePrefab;  // UI prefab to display above buildings, used in BuildingScore objects				
				public int BuildingScoreHeight;         // Height offset for BuildingScore UI element

				private PassengerController _PC;								// PassengerController
				private EntityController _EC;											// EntityController

				// BuildingScore UI objects by color
				private Dictionary<Building.BuildingColors, BuildingScore> _buildingScores;					


				#region Public Methods

				/// <summary>
				/// Links required Controllers to the ScoreController and initializes
				/// UI elements accordingly. Initialize() uses data from these controllers to
				/// construct the UI, so they MUST be initialized before passing them
				/// to the ScoreController's Initialize().
				/// </summary>
				public void Initialize(PassengerController PC, EntityController EC)
				{
								if (!PC.Initialized())
												throw new Exception("Score Controller was given an uninitialized Passenger Controller. " +
																"Did you forget to call PassengerController.Initialize() before passing the controller" +
																" to ScoreController.Initialize() ?");

								if (!EC.Initialized())
												throw new Exception("Score Controller was given an uninitialized Entity Controller. " +
																"Did you forget to call EntityController.Initialize() before passing the controller" +
																" to ScoreController.Initialize() ?");

								if (BuildingScorePrefab == null)
												throw new Exception("BuildingScorePrefab not set in ScoreController");

								_PC = PC;
								_EC = EC;
								_buildingScores = new Dictionary<Building.BuildingColors, BuildingScore>();
								Transform buildingScoreParent = this.gameObject.GetComponent<Transform>();

								Broadcaster.AddListener(GameEvent.PassengerDelivered, PassengerDelivered);
								Broadcaster.AddListener(GameEvent.BuildingComplete, BuildingComplete);

								foreach (Building building in _EC.Buildings)
								{
												Vector3 buildingScorePosition = new Vector3(building.transform.position.x, BuildingScoreHeight, building.transform.position.z);
												_buildingScores.Add(building.BuildingColor, new BuildingScore(building.BuildingColor, buildingScorePosition, 
																building.transform.rotation, buildingScoreParent, BuildingScorePrefab));
								}
				}
				#endregion

				#region Building Score Object

				/// <summary>
				/// This container acts as a wrapper for the logic required to update a 
				/// single building's "BuildingScore" prefab. Each BuildingScore object contains all
				/// necessary UI information & logic for updating it's own internal prefab. Data regarding
				/// passenger stats is pulled from this level's PassengerController and not stored internally.
				/// </summary>
				private class BuildingScore
    {
								ScoreController _SC;																// back reference to the parent ScoreController
								Building.BuildingColors _color;     // This building's color
								BuildingScoreState _state;          // This building's current completion state.
								GameObject _buildingScorePrefab;    // The UI prefab asociated with this building
								SpriteRenderer _icon;
								SpriteRenderer _star;
								TextMesh _playerScoreText;
								TextMesh _enemyScoreText;

								/// <summary>
								/// Creates a new BuildingScore object
								/// </summary>
								/// <param name="color"> Color associated with this building </param>
								/// <param name="position"> Position to place this BuildingScore prefab (typically above the building) </param>
								/// <param name="rotation"> Usually just the transform.rotation of the parent. 
								/// This is changed dynamically, so the initial value doesn't matter much </param>
								/// <param name="ScoreController"> Parent object to instantiate the BuildingScore under (usually ScoreController) </param>
								/// <param name="buildingScorePrefab"> Prefab that should be instantiated for the BuildingScore object. </param>
								public BuildingScore(Building.BuildingColors color, Vector3 position, Quaternion rotation, Transform ScoreController, GameObject buildingScorePrefab)
        {
												_SC = ScoreController.GetComponent<ScoreController>();
												_color = color;
            _state = BuildingScoreState.TBD;
												_buildingScorePrefab = Instantiate(buildingScorePrefab, position, rotation, ScoreController);
												_icon = _buildingScorePrefab.transform.Find("ColorIcon").GetComponent<SpriteRenderer>();
												_star = _buildingScorePrefab.transform.Find("Star").GetComponent<SpriteRenderer>();
												_playerScoreText = _buildingScorePrefab.transform.Find("PlayerScore").GetComponent<TextMesh>();
												_enemyScoreText = _buildingScorePrefab.transform.Find("EnemyScore").GetComponent<TextMesh>();
												_icon.material.color = Game.ColorKey.GetColor(_color);
												_star.material.color = Game.ColorKey.UIStarTBD;
        }

								/// <summary>
								/// Updates the scoring UI for this building
								/// </summary>
								/// <param name="PC"> The PassengerController associated with this level </param>
        public void UpdateDelivered(PassengerController PC)
        {
												if (_state == BuildingScoreState.TBD)
												{
																_playerScoreText.text = PC.GetPlayerPassengersDelivered(_color).ToString() + "/" + PC.GetPassengersRequired(_color).ToString();
																_enemyScoreText.text = PC.GetEnemyPassengersDelivered(_color).ToString() + "/" + PC.GetPassengersRequired(_color).ToString();
												}
								}

								/// <summary>
								/// Updates the star rating UI for this building.
								/// Returns the new state of the building
								/// </summary>
								/// <param name="PC"> The PassengerController associated with this level </param>
								public BuildingScoreState UpdateState(PassengerController PC)
        {
            if (!PC.GetSpawnStatus(_color))
            {
                if (PC.GetPlayerPassengersDelivered(_color) > PC.GetEnemyPassengersDelivered(_color))
                {
                    _state = BuildingScoreState.PlayerStar;
																				_star.material.color = Game.ColorKey.UIStarPlayer;
                }
                else
                {
                    _state = BuildingScoreState.EnemyStar;
																				_star.material.color = Game.ColorKey.UIStarEnemy;
																}
            }
												return _state;
        }
    }

				#endregion

				#region Event Handlers

				/// <summary>
				/// Handles the PassengerDelivered Broadcast.
				/// </summary>
				private void PassengerDelivered(GameEvent action)
    {
        foreach (BuildingScore building in _buildingScores.Values)
            building.UpdateDelivered(_PC);
    }

				/// <summary>
				/// Handles the BuildingComplete Broadcast.
				/// Checks for GameOver and broadcasts accordingly
				/// </summary>
    private void BuildingComplete(GameEvent action)
    {
								bool gameOver = true;			// true only if NONE of the buildings are still TBD (active)
								bool success = false;			// true if the player has AT LEAST 1 star
        foreach (BuildingScore building in _buildingScores.Values)
								{
												BuildingScoreState state = building.UpdateState(_PC);
												if (state == BuildingScoreState.TBD) { gameOver = false; }
												if (state == BuildingScoreState.PlayerStar) { success = true; }
								}

								if (gameOver)
								{
												if (success) { Debug.Log("Success Broadcasted!"); Broadcaster.Broadcast(GameEvent.LevelCompleteSuccess); }
												else { Debug.Log("Fail Broadcasted!"); Broadcaster.Broadcast(GameEvent.LevelCompleteFail); }
								}
    }

				#endregion

}