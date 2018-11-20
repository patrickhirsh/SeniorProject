using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "DebuggingProfile", menuName = "ParkingMaster/DebuggingProfile", order = 0)]
    [Serializable]
    public class DebuggingProfile : ScriptableObject
    {
        public bool DebugVehicle;
        public bool DebugPlayerVehicleManager;
    }
}