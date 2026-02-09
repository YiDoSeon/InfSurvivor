using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;

namespace Shared.Packet.Struct
{
    [MessagePackObject]
    public struct CVector2
    {
        [Key(0)] public float x;
        [Key(1)] public float y;
        private static readonly CVector2 zeroVector = new CVector2(0f, 0f);
        private static readonly CVector2 oneVector = new CVector2(1f, 1f);
        private static readonly CVector2 upVector = new CVector2(0f, 1f);
        private static readonly CVector2 downVector = new CVector2(0f, -1f);
        private static readonly CVector2 leftVector = new CVector2(-1f, 0f);
        private static readonly CVector2 rightVector = new CVector2(1f, 0f);
        public static CVector2 zero => zeroVector;
        public static CVector2 one => oneVector;
        public static CVector2 up => upVector;
        public static CVector2 down => downVector;
        public static CVector2 left => leftVector;
        public static CVector2 right => rightVector;
        [IgnoreMember] public readonly CVector2 normalized => Normalize(in this);
        [IgnoreMember] public readonly float magnitude => (float)Math.Sqrt(x * x + y * y);
        [IgnoreMember] public readonly float sqrMagnitude => x * x + y * y;

        public CVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static CVector2 Normalize(in CVector2 value)
        {
            float num = value.magnitude;
            return (num > 1E-05f) ? new CVector2
            {
                x = value.x / num,
                y = value.y / num
            } : zeroVector;
        }

        public static bool operator ==(CVector2 lhs, CVector2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(CVector2 lhs, CVector2 rhs)
        {
            return !(lhs == rhs);
        }

        public static CVector2 operator *(CVector2 lhs, float rhs)
        {
            return new CVector2(lhs.x * rhs, lhs.y * rhs);
        }

        public static CVector2 operator *(float rhs, CVector2 lhs)
        {
            return lhs * rhs;
        }

        public static CVector2 operator +(CVector2 lhs, CVector2 rhs)
        {
            return new CVector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static CVector2 operator -(CVector2 lhs, CVector2 rhs)
        {
            return new CVector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public override bool Equals(object obj)
        {
            return (CVector2)obj == this;
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