using UnityEngine;

namespace Level
{
    public class Terminal : MonoBehaviour
    {
        public Route ParentRoute => Connection.ParentRoute;
        public Connection Connection;
        public Passenger Passenger;

        public bool HasPassenger => Passenger != null;

        #region Unity Methods

        #endregion

        public void SpawnPassenger(Passenger prefab)
        {
            Passenger = Instantiate(prefab, ParentRoute.CenterTransform, false);
            Passenger.StartTerminal = this;
        }

        public void RemovePassenger()
        {
            Passenger = null;
        }
    }
}