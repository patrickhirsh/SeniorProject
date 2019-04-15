using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RideShareLevel
{
    public class PathNodeComparer : IComparer<PathNode>
    {
        public int Compare(PathNode node1, PathNode node2)
        {
            if (node1.Distance < node2.Distance)
                return -1;
            if (node1.Distance > node2.Distance)
                return 1;
            if (node1.Distance == node2.Distance)
                return 0;
            return 0;
        }
    }
}


