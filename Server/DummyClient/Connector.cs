using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Shared.Session;

namespace DummyClient
{
    public class Connector
    {
        private Func<SessionBase> sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<SessionBase> sessionFactory, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(
                    endPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                this.sessionFactory = sessionFactory;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);

                Thread.Sleep(10);
            }
        }

        private void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
            {
                return;
            }
            
            try
            {
                bool pending = socket.ConnectAsync(args);
                if (pending == false)
                {
                    OnConnectCompleted(null, args);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"{nameof(RegisterConnect)} Failed {e}");
            }
        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    SessionBase session = sessionFactory?.Invoke();
                    session.Start(args.ConnectSocket);
                    session.OnConnected(args.RemoteEndPoint);
                }
                else
                {
                    Console.WriteLine($"{nameof(OnConnectCompleted)} Failed {args.SocketError}");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"{nameof(OnConnectCompleted)} Failed {e}");
            }
        }
    }
}