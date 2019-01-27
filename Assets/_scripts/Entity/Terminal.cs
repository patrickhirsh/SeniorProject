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

        public bool SpawnPassenger(Passenger prefab)
        {
            if (!this.HasPassenger)
            {
                Passenger = Instantiate(prefab, ParentRoute.CenterTransform, false);
                Passenger.StartTerminal = this;
                return true;
            }
            return false;
        }

        public void RemovePassenger()
        {
            Passenger = null;
        }
    }
}