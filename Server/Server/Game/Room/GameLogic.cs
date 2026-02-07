using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Game.Job;
using Server.Game.Object;

namespace Server.Game.Room
{
    public class GameLogic : JobSerializer
    {
        private static GameLogic instance = new GameLogic();
        public static GameLogic Instance => instance;

        private Dictionary<int, GameRoom> rooms = new Dictionary<int, GameRoom>();

        private int roomId = 1;

        #region 프레임 관련
        private const int TICKS_PER_SEC = 30;
        private const float FIXED_DELTA_TIME = 1f / TICKS_PER_SEC;
        private Stopwatch sw = new Stopwatch();
        private long serverTime = 0;
        /// <summary>
        /// 게임 로직 Update가 호출된 시간부터의 시간 (ms) <br/>
        /// 외부 스레드에서 접근할 수 있으므로 lock처리
        /// </summary>
        /// <returns></returns>
        public long ServerTime
        {
            get
            {
                lock (_lock)
                {
                    return serverTime;
                }
            }
        }
        #endregion

        /// <summary>
        /// 고정 프레임 측정 스탑워치
        /// </summary>
        /// <returns></returns>
        private object _lock = new object();
        public void Update()
        {
            Stopwatch totalServerTime = Stopwatch.StartNew();

            sw.Start();

            long lastTimeMs = sw.ElapsedMilliseconds;
            double accumulator = 0d;

            while (true)
            {
                long nowMs = sw.ElapsedMilliseconds;
                double deltaMs = (nowMs - lastTimeMs) / 1000d;
                lastTimeMs = nowMs;

                accumulator += deltaMs;

                int tickCount = 0;

                while (accumulator >= FIXED_DELTA_TIME)
                {
                    long serverTime;
                    List<GameRoom> roomSnapshot;

                    lock (_lock)
                    {
                        serverTime = this.serverTime = totalServerTime.ElapsedMilliseconds;
                        // 스냅샷 저장 (lock해제를 빠르게 하기 위함)
                        roomSnapshot = rooms.Values.ToList();
                    }

                    foreach (GameRoom room in roomSnapshot)
                    {
                        room.Update(FIXED_DELTA_TIME, serverTime);
                    }

                    accumulator -= FIXED_DELTA_TIME;
                    tickCount++;

                    if (tickCount >= 5)
                    {
                        accumulator = 0;
                        break;
                    }
                }

                Thread.Sleep(0);
            }
        }

        public GameRoom Add()
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Push(gameRoom.Init);

            lock (_lock)
            {
                gameRoom.RoomId = roomId;
                rooms.Add(roomId, gameRoom);
                roomId++;
            }

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return rooms.Remove(roomId);
            }
        }

        public GameRoom Find(int roomId)
        {
            lock (_lock)
            {
                if (rooms.TryGetValue(roomId, out GameRoom room))
                {
                    return room;
                }
            }
            return null;
        }

        public void AddPlayerToRoom(Player player, int roomId)
        {
            GameRoom room = Find(roomId);
            if (room != null)
            {
                room.Push(room.EnterGame, player);
            }
        }

        public void RemovePlayerFromRoom(int playerId, int roomId)
        {
            GameRoom room = Find(roomId);
            if (room != null)
            {
                room.Push(room.LeaveGame, playerId);
            }
        }
    }
}