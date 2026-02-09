using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Packet;

namespace Server.Game.Object
{
    public class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }
    }
}