using Shared.Packet.Struct;

namespace Shared.Physics.Collider
{
    public class CBoxCollider : ColliderBase
    {
        public CVector2 Size { get; set; }
        public override ColliderType Type => ColliderType.Box;

        public CVector2 Min => Center - HalfSize;
        public CVector2 Max => Center + HalfSize;

        public override CVector2 HalfSize => Size * 0.5f;
        public CBoxCollider(IColliderTrigger trigger, CVector2 offset, CVector2 position, CVector2 size)
         : base(trigger, offset, position)
        {
            Size = size;
        }
    }
}
