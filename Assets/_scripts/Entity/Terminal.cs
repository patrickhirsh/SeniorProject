using UnityEngine;

namespace RideShareLevel
{
    public class Terminal : MonoBehaviour
    {
        public Route ParentRoute => Connection.ParentRoute;
        public Connection Connection;
        private Passenger _passenger;

        public bool HasPassenger => _passenger != null;

        #region Unity Methods

        #endregion

        public bool SetPassenger(Passenger passenger)
        {
            if (!this.HasPassenger)
            {
                _passenger = passenger;
                _passenger.transform.SetParent(ParentRoute.CenterTransform, false);
                _passenger.transform.localPosition = Vector3.zero;
                _passenger.StartTerminal = this;
                return true;
            }
            return false;
        }

        public void RemovePassenger()
        {
            _passenger = null;
        }

        public Passenger CurrentPassenger()
        {
            return _passenger;
        }
    }
}