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
    }
}
