using System;
using System.Collections;
using System.Collections.Generic;
using RideShareLevel;
using UnityEngine;

public enum BuildingScoreState { PlayerStar, EnemyStar, TBD };

public class ScoreController : LevelObject
{
				PassengerController PC;
				private Dictionary<Building.BuildingColors, BuildingScore> _buildingScores;

				public class BuildingScore
				{

								Building.BuildingColors _color { get; set; }
								BuildingScoreState _state { get; set; }

								public BuildingScore(Building.BuildingColors color)
								{
												_color = color;
												_state = BuildingScoreState.TBD;
								}

								public void UpdateDelivered(PassengerController PC)
								{
												PC.GetPlayerPassengersDelivered(_color);
												PC.GetEnemyPassengersDelivered(_color);
												PC.GetPassengersRequired(_color);
								}

								public void UpdateState(PassengerController PC)
								{
												if (!PC.GetSpawnStatus(_color))
												{
																if (PC.GetPlayerPassengersDelivered(_color) > PC.GetEnemyPassengersDelivered(_color))
																{
																				_state = BuildingScoreState.PlayerStar;
																}
																else
																{
																				_state = BuildingScoreState.EnemyStar;
																}
												}
								}
				}

				private void PassengerDelivered(GameEvent action)
				{
								foreach (BuildingScore building in _buildingScores.Values)
								{
												building.UpdateDelivered(PC);
								}
				}

				private void BuildingComplete(GameEvent action)
				{
								foreach(BuildingScore building in _buildingScores.Values)
								{
												building.UpdateState(PC);
								}
				}

				public void Initialize(PassengerController Controller)
				{
								PC = Controller;
								Dictionary<Building.BuildingColors, BuildingScore> _buildingScores = new Dictionary<Building.BuildingColors, BuildingScore>();

								Broadcaster.AddListener(GameEvent.PassengerDelivered, PassengerDelivered);
								Broadcaster.AddListener(GameEvent.BuildingComplete, BuildingComplete);

								foreach (Building.BuildingColors color in PC.GetBuildingColors())
								{
												_buildingScores.Add(color, new BuildingScore(color));
								}
				}
}