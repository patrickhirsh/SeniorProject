using Level;
using UnityEngine;

namespace Level
{
    public class Passenger : MonoBehaviour
    {
        public Terminal StartTerminal;
        public Terminal DestinationTerminal;

        public int SearchDepth;
        #region Unity Methods

        public void Start()
        {
            DestinationTerminal = PickRandomTerminal(SearchDepth);
        }

        #endregion

        private Terminal PickRandomTerminal(int searchDepth)
        {
            var depth = 0;
            var current = StartTerminal.ParentRoute;
            while (depth < searchDepth && !current.HasTerminals)
            {
                // Pick a random neighbor
                current = current.ConnectingRoutes[Random.Range(0, current.ConnectingRoutes.Length)];
                SearchDepth++;
            }

            // Pick a random terminal
            return current.Terminals[Random.Range(0, current.Terminals.Length)];
        }

    }
}