using System.Collections.Generic;
using Shared.Packet.Struct;
using Shared.Physics.Collider;

namespace Shared.Physics
{
    public class SpatialHashGrid
    {
        private float cellSize = 1.0f; // 한 칸의 크기
        public float CellSize => cellSize;
        private readonly Dictionary<CVector2Int, HashSet<ColliderBase>> gridData = new Dictionary<CVector2Int, HashSet<ColliderBase>>();
        public Dictionary<CVector2Int, HashSet<ColliderBase>> GridData => gridData;

        public SpatialHashGrid(float cellSize)
        {
            this.cellSize = cellSize;
        }

        public void AddToCells(HashSet<CVector2Int> coords, ColliderBase go)
        {
            if (go == null)
            {
                return;
            }
            foreach (CVector2Int pos in coords)
            {
                if (gridData.TryGetValue(pos, out HashSet<ColliderBase> set) == false)
                {
                    set = new HashSet<ColliderBase>();
                    gridData[pos] = set;
                }
                set.Add(go);
            }
        }

        public void RemoveFromCells(HashSet<CVector2Int> coords, ColliderBase go)
        {
            if (go == null)
            {
                return;
            }
            foreach (CVector2Int pos in coords)
            {
                if (gridData.TryGetValue(pos, out HashSet<ColliderBase> set))
                {
                    gridData[pos].Remove(go);
                }
            }
        }
    }
}
