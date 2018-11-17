using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Vehicle managers for all vehicle types should implement this class. 
    /// </summary>
    public abstract class VehicleManager : MonoBehaviour
    {
        /// <summary>
        /// Every vehicle manager needs to implement a callback function - called when a task
        /// it created has been terminated.
        /// </summary>
        /// <param name="type"> the taskType of the task that was terminated</param>
        /// <param name="vehicle"> the vehicle associated with the task </param>
        /// <param name="exitStatus"> true if the vehicle completed it's task. False if the task was interrupted </param>
        public abstract void vehicleTaskCallback(taskType type, Vehicle vehicle, bool exitStatus);
    }
}

