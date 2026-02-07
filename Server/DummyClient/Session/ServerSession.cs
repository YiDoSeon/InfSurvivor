using System;
using System.Net;
using Shared.Session;

namespace DummyClient.Session
{
    public class ServerSession : PacketSession
    {        
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("연결 성공!");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine("연결 해제!");
        }

        public override void OnSend(int numOfBytes)
        {
            
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
    }
}