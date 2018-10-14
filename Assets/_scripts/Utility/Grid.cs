using UnityEngine;

namespace Utility
{
    public static class Grid
    {
        public const float GridX = 1.5f;
        public const float GridY = 1;
        public const float GridZ = 1.5f;

        private const float GridHalfX = GridX / 2f;
        private const float GridHalfY = GridY / 2f;
        private const float GridHalfZ = GridZ / 2f;

        public static Vector3 GridSize => new Vector3(GridX, GridY, GridZ);

        public static Vector3 GetCellPos(Vector3 index)
        {
            return new Vector3(index.x * GridX, index.y * GridY, index.z * GridZ);
        }

        public static Vector3 GetCellCenter(Vector3 index)
        {
            var cellPos = GetCellPos(index);
            return new Vector3(cellPos.x + GridHalfX, cellPos.y + GridHalfY, cellPos.z + GridHalfZ);
        }

        public static Vector3 GetCellIndex(Vector3 position)
        {
            return new Vector3(Mathf.FloorToInt(position.x / GridX), Mathf.FloorToInt(position.y / GridY), Mathf.FloorToInt(position.z / GridZ));
        }

        public static Vector3 SnapToGrid(this Vector3 position)
        {
            return GetCellIndex(position);
        }

        public static void SnapToGrid(this Transform transform)
        {
            transform.position = GetCellPos(GetCellIndex(transform.position));
        }
    }
}