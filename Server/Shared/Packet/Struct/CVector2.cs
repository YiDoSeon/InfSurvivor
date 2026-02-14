using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CVector2 MoveTowards(CVector2 current, CVector2 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = num * num + num2 * num2;
            if (num3 == 0f || (maxDistanceDelta >= 0f && num3 <= maxDistanceDelta * maxDistanceDelta))
            {
                return target;
            }

            float num4 = (float)Math.Sqrt(num3);
            CVector2 result = default(CVector2);
            result.x = current.x + num / num4 * maxDistanceDelta;
            result.y = current.y + num2 / num4 * maxDistanceDelta;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CVector2 MoveTowards(in CVector2 current, in CVector2 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = num * num + num2 * num2;
            if (num3 == 0f || (maxDistanceDelta >= 0f && num3 <= maxDistanceDelta * maxDistanceDelta))
            {
                return target;
            }

            float num4 = (float)Math.Sqrt(num3);
            CVector2 result = default(CVector2);
            result.x = current.x + num / num4 * maxDistanceDelta;
            result.y = current.y + num2 / num4 * maxDistanceDelta;
            return result;
        }
    }
}