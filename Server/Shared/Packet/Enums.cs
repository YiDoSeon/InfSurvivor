namespace Shared.Packet
{
    public enum PacketId : ushort
    {
        S_CONNECTED,
        S_PING,
        C_PONG,
        S_ENTER_GAME,
        C_ENTER_GAME,
        S_SPAWN,
        S_DESPAWN,
        S_MOVE,
        C_MOVE,
        S_TIME_SYNC,
        C_TIME_SYNC,
        S_MELEE_ATTACK,
        C_MELEE_ATTACK,
    }

    public enum GameObjectType
    {
        Player,
        Monster,
    }

    public enum CollisionLayer
    {
        Default = 0,
        Player = 1,
        Monster = 2,
    }

    public enum PlayerState
    {
        Idle,
        Move,
        MeleeAttack,
    }

    public enum EnemyState
    {
        Idle,
        Move,
        Damaged,
    }
}