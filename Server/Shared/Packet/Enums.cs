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
    }

    public enum GameObjectType
    {
        Player,
        Monster,
    }
}