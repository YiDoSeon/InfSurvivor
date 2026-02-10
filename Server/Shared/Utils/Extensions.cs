using System.Linq;
using Shared.Packet;
using Shared.Packet.Struct;

namespace Shared.Utils
{
    public static class Extensions
    {
        #region GridPos
        public static CVector2Int ToGridPos(this CVector2 worldPos, float cellSize)
        {
            return new CVector2Int
            (
                CMath.FloorToInt(worldPos.x / cellSize),
                CMath.FloorToInt(worldPos.y / cellSize)
            );
        }
        #endregion

        public static int ToMask(this CollisionLayer layer)
        {
            return 1 << (int)layer;
        }

        public static int CombineMask(params CollisionLayer[] others)
        {
            // 전체를 0으로 시작하여 layer를 밀어 넣음
            return others.Aggregate(0, (current, layer) => current | layer.ToMask());
        }

        public static int ExcludeMask(params CollisionLayer[] excludes)
        {
            // 전체를 1로 시작하여 layer를 뺌
            return excludes.Aggregate(~0, (current, layer) => current & ~layer.ToMask());
        }
    }
}
