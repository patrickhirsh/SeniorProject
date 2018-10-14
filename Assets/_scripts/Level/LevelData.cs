using System;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "Level", menuName = "ParkingMaster/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        [Serializable]
        public struct NodeMapping
        {
            public int Index;
            public Node Prefab;
        }
        public int[,] NodeTypes;
        public NodeMapping[] NodeMappings;

    }
}