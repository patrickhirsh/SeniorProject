using System.Collections.Generic;
using System.Linq;
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
            var frontier = new HashSet<Route> { current };
            while (depth < searchDepth)
            {
                // Pick a random neighbor
                var available = current.NeighborRoutes.Where(route => !frontier.Contains(route)).ToArray();
                current = available[Random.Range(0, available.Length)];
                depth++;
            }

            while (!current.HasTerminals)
            {
                current = current.NeighborRoutes[Random.Range(0, current.NeighborRoutes.Length)];
            }

            // Pick a random terminal
            return current.Terminals[Random.Range(0, current.Terminals.Length)];
        }

        private void DrawPathToDestination()
        {
            Queue<Connection> connections;
            if (PathfindingManager.Instance.GetPath(StartTerminal.Connection, DestinationTerminal.Connection, out connections))
            {
                var curve = PathfindingManager.Instance.GeneratePath(connections);

            }
        }

    }
}