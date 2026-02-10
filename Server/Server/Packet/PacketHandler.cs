using System;
using Server.Game.Object;
using Server.Game.Room;
using Server.Session;
using Shared.Packet;
using Shared.Session;

public class PacketHandler
{
    public static void C_PongHandler(PacketSession session, IPacket packet)
    {
    }

    public static void C_EnterGameHandler(PacketSession session, IPacket packet)
    {
        C_EnterGame enterGamePacket = (C_EnterGame)packet;
        ClientSession clientSession = (ClientSession)session;

        clientSession.HandleEnterGame(enterGamePacket);
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = (C_Move)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        GameRoom room = player.Room;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleMove, player, movePacket);

        //Console.WriteLine($"PosX ({movePacket.PosInfo.PosX}) PosY ({movePacket.PosInfo.PosY})");
    }

    public static void C_TimeSyncHandler(PacketSession session, IPacket packet)
    {
        C_TimeSync timeSyncPacket = (C_TimeSync)packet;
        ClientSession clientSession = (ClientSession)session;

        clientSession.HandleTimeSync(timeSyncPacket);
    }

    internal static void C_MeleeAttackHandler(PacketSession session, IPacket packet)
    {
        C_MeleeAttack meleeAttackPacket = (C_MeleeAttack)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
        {
            return;
        }

        GameRoom room = player.Room;
        if (room == null)
        {
            return;
        }

        room.Push(room.HandleMeleeAttack, player, meleeAttackPacket);

    }
}