using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Game.Room;
using Shared.Packet;

namespace Server.Game.Object
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; set; }
        public GameRoom Room { get; set; }
        public ObjectInfo Info { get; set; } = new ObjectInfo() { PosInfo = new PositionInfo() };

        public int Id
        {
            get => Info.ObjectId;
            set => Info.ObjectId = value;
        }

        public PositionInfo PosInfo
        {
            get => Info.PosInfo;
            set => Info.PosInfo = value;
        }

        public CVector2 Pos
        {
            get => PosInfo.Pos;
            set => PosInfo.Pos = value;
        }

        public CVector2 Velocity
        {
            get => PosInfo.Velocity;
            set => PosInfo.Velocity = value;
        }
    }
}