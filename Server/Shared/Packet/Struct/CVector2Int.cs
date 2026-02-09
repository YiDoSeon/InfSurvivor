using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;

namespace Shared.Packet.Struct
{
    [MessagePackObject]
    public struct CVector2Int
    {
        [Key(0)] public int x;
        [Key(1)] public int y;

        private static readonly CVector2Int s_Zero = new CVector2Int(0, 0);
        private static readonly CVector2Int s_One = new CVector2Int(1, 1);
        private static readonly CVector2Int s_Up = new CVector2Int(0, 1);
        private static readonly CVector2Int s_Down = new CVector2Int(0, -1);
        private static readonly CVector2Int s_Left = new CVector2Int(-1, 0);
        private static readonly CVector2Int s_Right = new CVector2Int(1, 0);
        public static CVector2Int zero => s_Zero;
        public static CVector2Int one => s_One;
        public static CVector2Int up => s_Up;
        public static CVector2Int down => s_Down;
        public static CVector2Int left => s_Left;
        public static CVector2Int right => s_Right;
        [IgnoreMember] public readonly float magnitude => (float)Math.Sqrt(x * x + y * y);
        [IgnoreMember] public readonly float sqrMagnitude => x * x + y * y;
        public CVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(CVector2Int lhs, CVector2Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(CVector2Int lhs, CVector2Int rhs)
        {
            return !(lhs == rhs);
        }

        public static CVector2Int operator *(CVector2Int lhs, int rhs)
        {
            return new CVector2Int(lhs.x * rhs, lhs.y * rhs);
        }

        public static CVector2Int operator *(int rhs, CVector2Int lhs)
        {
            return lhs * rhs;
        }

        public static CVector2Int operator +(CVector2Int lhs, CVector2Int rhs)
        {
            return new CVector2Int(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static CVector2Int operator -(CVector2Int lhs, CVector2Int rhs)
        {
            return new CVector2Int(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public override bool Equals(object obj)
        {
            return (CVector2Int)obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

}