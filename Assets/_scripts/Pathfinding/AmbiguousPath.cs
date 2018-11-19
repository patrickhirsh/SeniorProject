using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Compares two AmbiguousPaths based on their weight
    /// </summary>
    public class AmbiguousPathComparer : IComparer<AmbiguousPath>
    {
        public int Compare(AmbiguousPath Path1, AmbiguousPath Path2)
        {
            if (Path1.weight < Path2.weight)
                return -1;
            if (Path1.weight > Path2.weight)
                return 1;
            if (Path1.weight == Path2.weight)
                return 0;

            else
                return 0;
        }
    }


    public class AmbiguousPath
    {
        public List<Connection> path;
        public float weight;


        /// <summary>
        /// Creates an empty AmbiguousPath
        /// </summary>
        public AmbiguousPath()
        {
            this.path = new List<Connection>();
            this.weight = 0;
        }

        /// <summary>
        /// Creates an Ambiguous path with connection path "path" and total edge weight "weight"
        /// </summary>
        public AmbiguousPath(List<Connection> path, float weight)
        {
            this.path = path;
            this.weight = weight;
        }

        /// <summary>
        /// Returns a deep copy of this AmbiguousPath
        /// </summary>
        public AmbiguousPath Copy()
        {
            AmbiguousPath output = new AmbiguousPath();
            foreach (Connection connection in this.path)
                output.path.Add(connection);

            output.weight = this.weight;
            return output;
        }


        /// <summary>
        /// Given a root AmbiguousPath and branches of AmbiguousPaths originating from the last connection in rootPath, merges each of 
        /// these AmbiguousPaths with the given rootPath. The result is a single AmbiguousPath that spans from the beginning of the 
        /// rootPath to the end of the branch path. The output "key" connection is set to the last connection in this merged path.
        /// </summary>
        public static IEnumerable<KeyValuePair<Connection, AmbiguousPath>> MergePaths(AmbiguousPath rootPath, Dictionary<Connection, AmbiguousPath> branches)
        {
            // for every branch, append it to the end of a copy of rootPath, assign the new total weight, and use branch's endConnection as the new key.
            foreach (KeyValuePair<Connection, AmbiguousPath> branch in branches)
            {
                // copy the rootPath, then add branch's path to it
                AmbiguousPath mergedPath = rootPath.Copy();
                foreach (Connection connection in branch.Value.path)
                    mergedPath.path.Add(connection);

                // the weight of this new merged path is the sum of these two paths
                mergedPath.weight += branch.Value.weight;

                // the final path reaches branch's end connection. Use that as the key
                yield return new KeyValuePair<Connection, AmbiguousPath>(branch.Key, mergedPath);           
            }
        }
    }
}

