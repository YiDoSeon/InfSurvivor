using System.Collections.Generic;
using Server.Game.Room;
using Shared.Packet;
using Shared.Packet.Struct;
using Shared.Physics.Collider;

namespace Server.Game.Object
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; }
        public GameRoom Room { get; set; }
        public ObjectInfo Info { get; set; } = new ObjectInfo()
        {
            PosInfo = new PositionInfo()
            {
                FacingDir = new CVector2(0f, -1f),
            }
        };
        protected HashSet<CVector2Int> occupiedCells = new HashSet<CVector2Int>();

        public virtual ColliderBase BodyCollider { get; }
        public float MoveSpeed { get; protected set; }
        public int Id
        {
            get => Info.ObjectId;
            set => Info.ObjectId = value;
        }

        public PositionInfo PosInfo
        {
            get => Info.PosInfo;
            protected set => Info.PosInfo = value;
        }

        public CVector2 Pos
        {
            get => PosInfo.Pos;
            protected set => PosInfo.Pos = value;
        }

        public CVector2 Velocity
        {
            get => PosInfo.Velocity;
            protected set => PosInfo.Velocity = value;
        }

        public CVector2 FacingDir
        {
            get => PosInfo.FacingDir;
            protected set => PosInfo.FacingDir = value;
        }

        public virtual void SetRoom(GameRoom room)
        {
            Room = room;
        }

        public virtual void InitPos(PositionInfo posInfo)
        {
            PosInfo = posInfo;
        }

        public virtual void OnTick()
        {
            BodyCollider?.UpdatePosition(Pos);
            Room.CollisionWorld.UpdateOccupiedCells(BodyCollider, occupiedCells);
        }
    }
}