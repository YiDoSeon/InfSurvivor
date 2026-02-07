using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Session
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

        private int sessionId = 0;
        private Dictionary<int, ClientSession> sessions = new Dictionary<int, ClientSession>();
        private object _lock = new object();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++this.sessionId;

                ClientSession session = new ClientSession(sessionId);
                sessions.Add(sessionId, session);

                Console.WriteLine($"Connected ({sessions.Count}) Players");

                return session;
            }
        }

        public List<ClientSession> GetSessions()
        {
            List<ClientSession> sessions = new List<ClientSession>();

            lock (_lock)
            {
                sessions = this.sessions.Values.ToList();
            }
            return sessions;
        }

        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                if (sessions.TryGetValue(id, out ClientSession session))
                {
                    return session;
                }
                return null;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Remove(session.SessionId);
                Console.WriteLine($"Connected ({sessions.Count}) Players");
            }
        }
    }
}