using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Server.Game.Room;
using Server.Session;
using Shared;

namespace Server;

class Program
{

    private static void NetworkTask()
    {
        while (true)
        {
            List<ClientSession> sessions = SessionManager.Instance.GetSessions();
            foreach (ClientSession session in sessions)
            {
                session.FlushSend();
            }
            Thread.Sleep(0);
        }
    }

    private static void Main(string[] args)
    {
        // TODO: 공용 폴더등에서 사용할 수 있도록
        // string connectionString = "Data Source=MyDatabase.db";

        // using (SqliteConnection connection = new SqliteConnection(connectionString))
        // {
        //     connection.Open();

        //     connection.Close();
        // }

        // GameLogic.Instance.Push(() =>
        // {
        //     GameRoom room =
        // });
        GameLogic.Instance.Add();

        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 7777);

        Listener listener = new Listener();
        listener.Init(endPoint, SessionManager.Instance.Generate);

        Console.WriteLine("서버 시작!");

        {
            Thread t = new Thread(NetworkTask);
            t.Name = "Network";
            t.Start();            
        }

        Thread.CurrentThread.Name = "GameLogic";
        GameLogic.Instance.Update();
    }
}