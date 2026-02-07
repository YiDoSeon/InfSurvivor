using System;
using System.Collections.Generic;
using Shared.Packet;

namespace DummyClient.Session
{
    public class SessionManager
    {
        private static SessionManager instance = new SessionManager();
        public static SessionManager Instance
        {
            get
            {
                return instance;
            }
        }

        private List<ServerSession> sessions = new List<ServerSession>();
        private Random rand = new Random();
        private object _lock = new object();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                sessions.Add(session);
                return session;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                sessions.Clear();
            }
        }

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in sessions)
                {
                    C_SimpleMsg msgPacket = new C_SimpleMsg();
                    msgPacket.Msg.MsgList.Add("Hello Server!");
                    session.Send(msgPacket);
                }
            }
        }
    }
}