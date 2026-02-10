
using System;
using System.Collections.Generic;
using MessagePack;
using Shared.Session;
using Shared.Packet;

public class PacketManager
{
    #region Singleton
    private static PacketManager instance = new PacketManager();
    public static PacketManager Instance => instance;
    #endregion

    public PacketManager()
    {
        Register();
    }

    #region delegate
    public delegate void RawPacketHandler(PacketSession session, ArraySegment<byte> buffer, PacketId id);
    public delegate void PacketProcessor(PacketSession session, IPacket packet);
    public delegate void PacketInterceptor(PacketSession session, IPacket packet, PacketId id);
    #endregion

    private Dictionary<PacketId, RawPacketHandler> rawPacketHandlers = new Dictionary<PacketId, RawPacketHandler>();
    private Dictionary<PacketId, PacketProcessor> packetProcessors = new();

    public PacketInterceptor CustomPacketInterceptor { get; set; }

    public void Register()
    {
        
        rawPacketHandlers.Add(PacketId.C_PONG, MakePacket<C_Pong>);
        packetProcessors.Add(PacketId.C_PONG, PacketHandler.C_PongHandler);
        rawPacketHandlers.Add(PacketId.C_ENTER_GAME, MakePacket<C_EnterGame>);
        packetProcessors.Add(PacketId.C_ENTER_GAME, PacketHandler.C_EnterGameHandler);
        rawPacketHandlers.Add(PacketId.C_MOVE, MakePacket<C_Move>);
        packetProcessors.Add(PacketId.C_MOVE, PacketHandler.C_MoveHandler);
        rawPacketHandlers.Add(PacketId.C_TIME_SYNC, MakePacket<C_TimeSync>);
        packetProcessors.Add(PacketId.C_TIME_SYNC, PacketHandler.C_TimeSyncHandler);
        rawPacketHandlers.Add(PacketId.C_MELEE_ATTACK, MakePacket<C_MeleeAttack>);
        packetProcessors.Add(PacketId.C_MELEE_ATTACK, PacketHandler.C_MeleeAttackHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        PacketId id = (PacketId)BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (rawPacketHandlers.TryGetValue(id, out RawPacketHandler action))
        {
            action.Invoke(session, buffer, id);
        }
    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, PacketId id)
        where T : IPacket
    {
        T pkt = MessagePackSerializer.Deserialize<T>
            (
                new ReadOnlyMemory<byte>
                (
                    buffer.Array,
                    buffer.Offset + 4,
                    buffer.Count - 4
                )
            );

        if (CustomPacketInterceptor != null)
        {
            CustomPacketInterceptor.Invoke(session, pkt, id);
        }
        else
        {
            GetPacketProcessor(id)?.Invoke(session, pkt);
        }
    }

    public PacketProcessor GetPacketProcessor(PacketId id)
    {
        packetProcessors.TryGetValue(id, out PacketProcessor action);
        return action;
    }
}
