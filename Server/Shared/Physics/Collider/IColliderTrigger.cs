namespace Shared.Physics.Collider
{
    public interface IColliderTrigger
    {
        void OnCustomTriggerEnter(ColliderBase other);
        void OnCustomTriggerStay(ColliderBase other);
        void OnCustomTriggerExit(ColliderBase other);
    }
}
