using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RideShareLevel
{
    public class PathNodeComparer : IComparer<PathNode>
    {
        public int Compare(PathNode node1, PathNode node2)
        {
            if (node1.distance < node2.distance)
                return -1;
            else if (node1.distance > node2.distance)
                return 1;
            else if (node1.distance == node2.distance)
                return 0;

            else return 0;
        }
    }
}


