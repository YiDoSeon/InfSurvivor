using System;
using System.Net;
using System.Threading;
using DummyClient.Session;

namespace DummyClient;

class Program
{
    private static void Main(string[] args)
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint, SessionManager.Instance.Generate);

        while (true)
        {
            Console.ReadKey(true);
            SessionManager.Instance.SendForEach();
            Thread.Sleep(100);
        }
    }
}