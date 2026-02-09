using System;
using System.Collections.Generic;
using System.Net;
using Server.Game.Object;
using Server.Game.Room;
using Shared.Packet;
using Shared.Session;

namespace Server.Session
{
    public class ClientSession : PacketSession
    {
        private const int FLUSH_MS = 33;
        public Player MyPlayer { get; set; }
        public int SessionId { get; }
        private object _lock = new object();

        private List<ArraySegment<byte>> reserveQueue = new List<ArraySegment<byte>>();
        private int reservedSendBytes = 0;
        private long lastSendTick = 0;

        public ClientSession(int sessionId)
        {
            SessionId = sessionId;
        }
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"[연결] {endPoint}");

            S_Connected connectedPacket = new S_Connected();
            connectedPacket.Ok = true;
            Send(connectedPacket);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Player player = MyPlayer;
            if (player != null)
            {
                GameLogic.Instance.RemovePlayerFromRoom(player.Id, 1);                
            }
            SessionManager.Instance.Remove(this);
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"[송신 완료] {numOfBytes} bytes");
        }

        public override void SendBufferImpl(ArraySegment<byte> sendBuffer)
        {
            lock (_lock)
            {
                ushort id = BitConverter.ToUInt16(sendBuffer.Array, sendBuffer.Offset + 2);
#if DEBUG_INTERNAL
                Console.WriteLine($"[송신2]: {(PacketId)id}");
#endif
                reserveQueue.Add(sendBuffer);
                reservedSendBytes += sendBuffer.Count;
            }
        }

        public void FlushSend()
        {
            List<ArraySegment<byte>> sendList = null;

            lock (_lock)
            {
                long delta = Environment.TickCount64 - lastSendTick;
                if (delta < FLUSH_MS && reservedSendBytes < 10000)
                {
                    return;
                }

                reservedSendBytes = 0;
                lastSendTick = Environment.TickCount64;

                sendList = reserveQueue;
                reserveQueue = new List<ArraySegment<byte>>();
            }

            SendBuffer(sendList);
        }

        public void HandleEnterGame(C_EnterGame enterGamePacket)
        {
            Console.WriteLine(enterGamePacket.Name);

            MyPlayer = ObjectManager.Instance.Add<Player>();
            {
                MyPlayer.Info.Name = enterGamePacket.Name;
                MyPlayer.Session = this;
            }

            GameLogic.Instance.AddPlayerToRoom(MyPlayer, 1);
        }

        public void HandleTimeSync(C_TimeSync clientTimeSyncPacket)
        {
            S_TimeSync timeSyncPacket = new S_TimeSync();
            timeSyncPacket.ClientTime = clientTimeSyncPacket.ClientTime;
            timeSyncPacket.ServerTime = GameLogic.Instance.ServerTime;

            Send(timeSyncPacket);
        }
    }
}