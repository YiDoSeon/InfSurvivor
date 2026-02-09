using Shared.Packet.Struct;

namespace Shared.Physics.Collider
{
    public class CCircleCollider : ColliderBase
    {
        public float Radius { get; set; }
        public override ColliderType Type => ColliderType.Circle;
        public override CVector2 HalfSize => new CVector2(Radius, Radius);
        public CCircleCollider(IColliderTrigger trigger, CVector2 offset, CVector2 position, float radius)
         : base(trigger, offset, position)
        {
            Radius = radius;
        }
    }
}
