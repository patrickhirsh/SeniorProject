using System;
using UnityEngine;
using Utility;

namespace Level
{
    [CreateAssetMenu(fileName = "Level", menuName = "ParkingMaster/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        [Serializable]
        public struct EntityMapping
        {
            public CellIndex Index;
            public Entity Prefab;
        }
    }
}