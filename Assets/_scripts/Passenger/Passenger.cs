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

        public GameObject PassengerDestinationReticle;
        public Vector3 AdjustmentVector;
        #region Unity Methods

        public void Start()
        {
            
            DestinationTerminal = PickRandomTerminal(SearchDepth);

        }

        #endregion

        public void SpawnDestinationReticle()
        {
            var reticle = Instantiate(PassengerDestinationReticle, DestinationTerminal.ParentRoute.transform.position + AdjustmentVector, Quaternion.identity, DestinationTerminal.transform);
        }

        private Terminal PickRandomTerminal(int searchDepth)
        {
            var routes = EntityManager.Instance.Routes.Where(route => route.HasTerminals).ToArray();
            var terminals = routes[Random.Range(0, routes.Length)].Terminals;
            var terminal = terminals[Random.Range(0, terminals.Length)];
            if (terminal != StartTerminal) return terminal;
            return PickRandomTerminal(searchDepth);

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