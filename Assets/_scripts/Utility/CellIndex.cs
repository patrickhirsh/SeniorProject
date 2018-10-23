﻿using UnityEngine;

namespace Utility
{
    public struct CellIndex
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public CellIndex(float vectorX, float vectorY, float vectorZ)
        {
            x = Mathf.RoundToInt(vectorX);
            y = Mathf.RoundToInt(vectorY);
            z = Mathf.RoundToInt(vectorZ);
        }

        public CellIndex(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Converts a CellIndex to a Vector3
        /// </summary>
        public static explicit operator Vector3(CellIndex index)
        {
            return new Vector3(index.x, index.y, index.z);
        }

        /// <summary>
        /// Converts a Vector3 to a CellIndex
        /// </summary>
        public static explicit operator CellIndex(Vector3 vector)
        {
            return new CellIndex(vector.x, vector.y, vector.z);
        }

        public static CellIndex Infinity()
        {
            return new CellIndex(int.MaxValue, int.MaxValue, int.MaxValue);
        }
    }
}