using UnityEngine;

namespace Utility
{
    public static class Grid
    {
        public const float CellSizeX = 1.5f;
        public const float CellSizeY = 1;
        public const float CellSizeZ = 1.5f;

        public const float CellHalfSizeX = CellSizeX / 2f;
        public const float CellHalfSizeY = CellSizeY / 2f;
        public const float CellHalfSizeZ = CellSizeZ / 2f;

        public static CellIndex CellSize => new CellIndex(CellSizeX, CellSizeY, CellSizeZ);

        public static Vector3 GetCellPos(CellIndex index)
        {
            return new Vector3(index.x * CellSizeX, index.y * CellSizeY, index.z * CellSizeZ);
        }

        public static Vector3 GetCellPosPlusHalf(CellIndex index)
        {
            var cellPos = GetCellPos(index);
            return new Vector3(cellPos.x + CellHalfSizeX, cellPos.y + CellHalfSizeY, cellPos.z + CellHalfSizeZ);
        }

        public static CellIndex GetCellIndex(Vector3 position)
        {
            return new CellIndex(Mathf.RoundToInt(position.x / CellSizeX), Mathf.RoundToInt(position.y / CellSizeY), Mathf.RoundToInt(position.z / CellSizeZ));
        }

        public static CellIndex CellIndex(this Transform transform)
        {
            return GetCellIndex(transform.position);
        }

        public static Vector3 GetPosition(this CellIndex index)
        {
            return Grid.GetCellPos(index);
        }

        public static void SnapToGrid(this Transform transform)
        {
            transform.position = GetCellPos(GetCellIndex(transform.position));
        }
    }
}