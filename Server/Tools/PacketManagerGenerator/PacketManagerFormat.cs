
namespace PacketManagerGenerator;

public class PacketFormat
{
    public static string managerFormat =
@"
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
        {REGISTER_BODY}
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
";

    public static string managerRegisterFormat =
@"
        rawPacketHandlers.Add(PacketId.{0}, MakePacket<{1}>);
        packetProcessors.Add(PacketId.{0}, PacketHandler.{1}Handler);";
}

