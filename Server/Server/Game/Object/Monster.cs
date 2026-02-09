using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Game.Room;
using Shared.Packet;
using Shared.Packet.Struct;
using Shared.Physics.Collider;

namespace Server.Game.Object
{
    public class Monster : GameObject, IColliderTrigger
    {
        private CCircleCollider bodyCollider;
        public override ColliderBase BodyCollider => bodyCollider;
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
            bodyCollider = new CCircleCollider(
                this,
                CVector2.zero,
                Pos,
                0.5f);
            bodyCollider.Layer = CollisionLayer.Monster;
        }

        public override void SetRoom(GameRoom room)
        {
            base.SetRoom(room);
            Room.CollisionWorld.RegisterCollider(bodyCollider);
        }

        public void OnCustomTriggerEnter(ColliderBase other)
        {
            if (other.Owner is Player player)
            {
                Console.WriteLine(player.Info.Name);
            }
        }

        public void OnCustomTriggerExit(ColliderBase other)
        {
            
        }

        public void OnCustomTriggerStay(ColliderBase other)
        {
            
        }
    }
}