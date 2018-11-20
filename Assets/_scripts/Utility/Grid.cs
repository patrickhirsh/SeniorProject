using UnityEngine;

namespace Utility
{
    public static class Grid
    {
        public static float CELL_SIZE_X = 1.5f * GameManager.Instance.Scale;
        public static float CELL_SIZE_Y = 1 * GameManager.Instance.Scale;
        public static float CELL_SIZE_Z = 1.5f * GameManager.Instance.Scale;

        public static float CELL_HALF_X = CELL_SIZE_X / 2f;
        public static float CELL_HALF_Y = CELL_SIZE_Y / 2f;
        public static float CELL_HALF_Z = CELL_SIZE_Z / 2f;

        public static CellIndex CellSize => new CellIndex(CELL_SIZE_X, CELL_SIZE_Y, CELL_SIZE_Z);

        public static Vector3 GetCellPos(CellIndex index)
        {
            return new Vector3(index.x * CELL_SIZE_X, index.y * CELL_SIZE_Y, index.z * CELL_SIZE_Z);
        }

        public static Vector3 GetCellPosPlusHalf(CellIndex index)
        {
            var cellPos = GetCellPos(index);
            return new Vector3(cellPos.x + CELL_HALF_X, cellPos.y + CELL_HALF_Y, cellPos.z + CELL_HALF_Z);
        }

        public static CellIndex GetCellIndex(Vector3 position)
        {
            return new CellIndex(Mathf.RoundToInt(position.x / CELL_SIZE_X), Mathf.RoundToInt(position.y / CELL_SIZE_Y), Mathf.RoundToInt(position.z / CELL_SIZE_Z));
        }

        public static CellIndex CellIndex(this Transform transform)
        {
            return GetCellIndex(transform.position);
        }

        public static Vector3 CellPosition(this Transform transform)
        {
            return transform.CellIndex().GetPosition();
        }

        public static Vector3 GetPosition(this CellIndex index)
        {
            return GetCellPos(index);
        }

        public static void SnapToGrid(this Transform transform)
        {
            transform.position = GetCellPos(GetCellIndex(transform.position));
        }
    }
}