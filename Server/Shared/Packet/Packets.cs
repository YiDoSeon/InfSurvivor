using System;
using System.Collections.Generic;
using MessagePack;

namespace Shared.Packet
{
    public interface IPacket
    {
        PacketId PacketId { get; }
    }

    [MessagePackObject]
    public struct CVector2
    {
        [Key(0)] public float x;
        [Key(1)] public float y;
        public CVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
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

    [MessagePackObject]
    public class ObjectInfo
    {
        [Key(0)] public int ObjectId { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public PositionInfo PosInfo { get; set; }

        public void MergeFrom(ObjectInfo other)
        {
            if (other.ObjectId != 0)
                ObjectId = other.ObjectId;
            if (other.Name.Length != 0)
                Name = other.Name;
            if (other.PosInfo != null)
            {
                PosInfo ??= new PositionInfo();
                PosInfo.MergeFrom(other.PosInfo);
            }
        }
    }

    [MessagePackObject]
    public class PositionInfo
    {
        [Key(0)] public CVector2 Pos { get; set; }
        [Key(1)] public CVector2 Velocity { get; set; }
        [Key(2)] public CVector2 FacingDir { get; set; }
        [Key(3)] public bool FirePressed { get; set; }

        public void MergeFrom(PositionInfo other)
        {
            if (other.Pos != default)
                Pos = other.Pos;
            if (other.Velocity != default)
                Velocity = other.Velocity;
            if (other.FacingDir != default)
                FacingDir = other.FacingDir;
            if (other.FirePressed != false)
                FirePressed = other.FirePressed;
        }
    }

    #region Connect
    [MessagePackObject]
    public class S_Connected : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.S_CONNECTED;
        [Key(0)] public bool Ok { get; set; }
    }
    #endregion

    #region Ping Pong
    [MessagePackObject]
    public class S_Ping : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.S_PING;

        [Key(0)] public int Value { get; set; }
    }

    [MessagePackObject]
    public class C_Pong : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.C_PONG;

        [Key(0)] public int Value { get; set; }
    }
    #endregion

    #region EnterGame

    [MessagePackObject]
    public class S_EnterGame : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.S_ENTER_GAME;
        [Key(0)] public ObjectInfo Player { get; set; } = new ObjectInfo();
    }

    [MessagePackObject]
    public class C_EnterGame : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.C_ENTER_GAME;
        [Key(0)] public string Name { get; set; }
    }
    #endregion

    #region Spawn
    [MessagePackObject]
    public class S_Spawn : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.S_SPAWN;
        [Key(0)] public List<ObjectInfo> Objects { get; set; } = new List<ObjectInfo>();
    }
    [MessagePackObject]
    public class S_Despawn : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.S_DESPAWN;
        [Key(0)] public List<int> ObjectIds { get; set; } = new List<int>();
    }
    #endregion

    #region Move
    [MessagePackObject]
    public class S_Move : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.S_MOVE;
        [Key(0)] public int ObjectId { get; set; }
        [Key(1)] public long ClientTime { get; set; }
        [Key(2)] public uint SeqNumber { get; set; }
        [Key(3)] public PositionInfo PosInfo { get; set; } = new PositionInfo();
        public void MergeFrom(S_Move other)
        {
            if (other.ObjectId != 0)
                ObjectId = other.ObjectId;
            if (other.ClientTime != 0)
                ClientTime = other.ClientTime;
            if (other.SeqNumber != 0)
                SeqNumber = other.SeqNumber;
            if (other.PosInfo != null)
            {
                PosInfo ??= new PositionInfo();
                PosInfo.MergeFrom(other.PosInfo);
            }
        }
    }

    [MessagePackObject]
    public class C_Move : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.C_MOVE;
        [Key(0)] public uint SeqNumber { get; set; }
        [Key(1)] public long ClientTime { get; set; }
        [Key(2)] public PositionInfo PosInfo { get; set; } = new PositionInfo();

        public void MergeFrom(C_Move other)
        {
            if (other.SeqNumber != 0)
                SeqNumber = other.SeqNumber;
            if (other.ClientTime != 0)
                ClientTime = other.ClientTime;
            if (other.PosInfo != null)
            {
                PosInfo ??= new PositionInfo();
                PosInfo.MergeFrom(other.PosInfo);
            }
        }
    }
    #endregion

    #region TimeSync

    [MessagePackObject]
    public class S_TimeSync : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.S_TIME_SYNC;
        [Key(0)] public long ClientTime { get; set; }
        [Key(1)] public long ServerTime { get; set; }
        public void MergeFrom(S_TimeSync other)
        {
            if (other.ClientTime != 0)
                ClientTime = other.ClientTime;
            if (other.ServerTime != 0)
                ServerTime = other.ClientTime;
        }
    }

    [MessagePackObject]
    public class C_TimeSync : IPacket
    {
        [IgnoreMember] public PacketId PacketId => PacketId.C_TIME_SYNC;
        [Key(0)] public long ClientTime { get; set; }
        public void MergeFrom(C_TimeSync other)
        {
            if (other.ClientTime != 0)
                ClientTime = other.ClientTime;
        }
    }
    #endregion
}