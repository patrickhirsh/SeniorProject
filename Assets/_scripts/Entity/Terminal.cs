using UnityEngine;

namespace Level
{
    public class Terminal : MonoBehaviour
    {
        public Route ParentRoute;
        public Passenger Passenger;

        public bool HasPassenger => Passenger != null;
        
        #region Unity Methods

        private void Awake()
        {
            if (ParentRoute == null)
            {
                ParentRoute = transform.GetComponentInParent<Route>();
            }
        }

        #endregion

        public void SpawnPassenger(Passenger prefab)
        {
            Passenger = Instantiate(prefab, transform.position, Quaternion.identity);
            Passenger.StartTerminal = this;
        }
    }
}