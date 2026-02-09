using System.Collections.Generic;
using Shared.Packet;
using Shared.Packet.Struct;
using Shared.Utils;

namespace Shared.Physics.Collider
{
    public enum ColliderType
    {
        Circle,
        Box,
    }

    public abstract class ColliderBase
    {
        public CollisionLayer Layer { get; set; } = CollisionLayer.Default;
        public CVector2 Offset { get; set; }
        public CVector2 Position { get; set; }
        public abstract ColliderType Type { get; }
        public CVector2 Center => Position + Offset;
        public CVector2Int LastMinGrid { get; set; }
        public CVector2Int LastMaxGrid { get; set; }
        public HashSet<IColliderTrigger> OverlappingOwners = new HashSet<IColliderTrigger>();
        private readonly IColliderTrigger owner;
        public IColliderTrigger Owner => owner;

        public ColliderBase(IColliderTrigger trigger, CVector2 offset, CVector2 position)
        {
            this.owner = trigger;
            Offset = offset;
            Position = position;
        }

        public abstract CVector2 HalfSize { get; }

        public void SetOffset(CVector2 offset)
        {
            Offset = offset;
        }

        public void UpdatePosition(CVector2 position)
        {
            Position = position;
        }

        public bool CheckCollision(ColliderBase other) 
            => (this.Type, other.Type) switch
        {
            (ColliderType.Circle, ColliderType.Circle) 
                => CollisionMath.CircleVsCircle((CCircleCollider)this, (CCircleCollider)other),
            (ColliderType.Box, ColliderType.Box) 
                => CollisionMath.BoxVsBox((CBoxCollider)this, (CBoxCollider)other),
            (ColliderType.Circle, ColliderType.Box) 
                => CollisionMath.CircleVsBox((CCircleCollider)this, (CBoxCollider)other),
            (ColliderType.Box, ColliderType.Circle) 
                => CollisionMath.CircleVsBox((CCircleCollider)other, (CBoxCollider)this),
            _ => false
        };
    }
}
