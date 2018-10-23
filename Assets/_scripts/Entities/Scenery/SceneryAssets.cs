using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "SceneryData", menuName = "ParkingMaster/SceneryData", order = 1)]
    public class SceneryAssets : ScriptableObject
    {
        public List<GameObject> Assets = new List<GameObject>();
    }
}