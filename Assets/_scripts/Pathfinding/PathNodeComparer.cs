using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class PathNodeComparer : IComparer<PathNode>
    {
        public int Compare(PathNode node1, PathNode node2)
        {
            if (node1.weight < node2.weight)
                return -1;
            else if (node1.weight > node2.weight)
                return 1;
            else if (node1.weight == node2.weight)
                return 0;

            else return 0;
        }
    }
}


