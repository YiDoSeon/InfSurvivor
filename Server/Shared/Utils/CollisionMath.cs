using Shared.Physics.Collider;

namespace Shared.Utils
{
    public static class CollisionMath
    {
        public static bool CircleVsCircle(CCircleCollider a, CCircleCollider b)
        {
            float radiusSum = a.Radius + b.Radius;
            float distSq = (a.Center - b.Center).sqrMagnitude;
            return distSq <= (radiusSum * radiusSum);
        }

        public static bool BoxVsBox(CBoxCollider a, CBoxCollider b)
        {
            return (a.Min.x <= b.Max.x && a.Max.x >= b.Min.x) &&
               (a.Min.y <= b.Max.y && a.Max.y >= b.Min.y);
        }

        public static bool CircleVsBox(CCircleCollider circle, CBoxCollider box)
        {
            float targetX = circle.Center.x;
            float targetY = circle.Center.y;

            float closestX = CMath.Clamp(targetX, box.Min.x, box.Max.x);
            float closestY = CMath.Clamp(targetY, box.Min.y, box.Max.y);

            float dx = targetX - closestX;
            float dy = targetY - closestY;

            return (dx * dx + dy * dy) <= (circle.Radius * circle.Radius);
        }
    }
}
