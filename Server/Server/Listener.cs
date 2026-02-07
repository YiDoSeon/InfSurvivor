using System;
using System.Net;
using System.Net.Sockets;
using Shared.Session;

namespace Server
{
    public class Listener
    {
        private Socket listenSocket;
        private Func<SessionBase> sessionFactory;

        public void Init(IPEndPoint endPoint, Func<SessionBase> sessionFactory)
        {
            listenSocket = new Socket(
                endPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            this.sessionFactory = sessionFactory;

            listenSocket.Bind(endPoint);

            listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAccept(args);
        }

        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            try
            {
                bool pending = listenSocket.AcceptAsync(args);

                if (pending == false)
                {
                    OnAcceptCompleted(null, args);
                }                
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"{nameof(RegisterAccept)} Failed {e}");
            }

        }
        
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    SessionBase session = sessionFactory?.Invoke();
                    session.Start(args.AcceptSocket);
                    session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                }
                else
                {
                    Console.WriteLine($"{nameof(OnAcceptCompleted)} Failed {args.SocketError}");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"{nameof(OnAcceptCompleted)} Failed {e}");
            }

            RegisterAccept(args);
        }
    }
}