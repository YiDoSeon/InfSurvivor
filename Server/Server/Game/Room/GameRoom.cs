using System;
using System.Collections.Generic;
using System.Diagnostics;
using Server.Game.Job;
using Server.Game.Object;
using Server.Session;
using Shared.Packet;
using Shared.Packet.Struct;
using Shared.Physics;

namespace Server.Game.Room
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private Dictionary<int, Enemy> monsters = new Dictionary<int, Enemy>();
        public long LastServerTime { get; private set; }
        public CollisionWorld CollisionWorld { get; private set; } = new CollisionWorld();

        public void Init()
        {
            CVector2 offset = new CVector2(1.4f, -8f);
            Random rand = new Random();
            // float[] randX = new float[2] {-20f, 20f};
            // float[] randY = new float[2] {-11f, 11f};
            // float[] randX = new float[2] {-20f, 20f};
            // float[] randY = new float[2] {-20f, 20f};
            float[] randX = new float[2] {-5f, 5f};
            float[] randY = new float[2] {-5f, 5f};
            for (int i = 0; i < 10; i++)
            {
                Enemy monster = ObjectManager.Instance.Add<Enemy>();
                {
                    monster.Info.Name = $"Monster-{i}";
                    monster.SetRoom(this);
                    monster.InitPos(new PositionInfo()
                    {
                        Pos = new CVector2(
                            offset.x + rand.NextSingle() * (randX[1] - randX[0]) + randX[0],
                            offset.y + rand.NextSingle() * (randY[1] - randY[0]) + randY[0])
                    });
                    EnterGame(monster);
                }
            }

            CollisionWorld.Init();
        }

        public void Update(long serverTime)
        {
            LastServerTime = serverTime;
            Flush();

            //Stopwatch sww = new Stopwatch();
            foreach (Player player in players.Values)
            {
                player.OnTick();
            }
            foreach (Enemy monster in monsters.Values)
            {
                monster.OnTick();
            }

            CollisionWorld.OnTick();
            //Console.WriteLine(sww.Elapsed.TotalMicroseconds);
        }

        public void BroadCast<T>(T packet) where T : IPacket
        {
            List<ClientSession> sessions = SessionManager.Instance.GetSessions();
            foreach (ClientSession session in sessions)
            {
                session.Send(packet);
            }
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                Player player = (Player)gameObject;
                players.Add(gameObject.Id, player);
                player.SetRoom(this);

                {
                    S_EnterGame enterGamePacket = new S_EnterGame
                    {
                        Player = player.Info
                    };
                    player.Session.Send(enterGamePacket);
                }

                // 본인에게 타인들 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in players.Values)
                    {
                        ObjectInfo info = new ObjectInfo();
                        info.MergeFrom(p.Info);
                        spawnPacket.Objects.Add(info);
                    }
                    foreach (Enemy m in monsters.Values)
                    {
                        ObjectInfo info = new ObjectInfo();
                        info.MergeFrom(m.Info);
                        spawnPacket.Objects.Add(info);
                    }
                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Enemy monster = (Enemy)gameObject;
                monsters.Add(gameObject.Id, monster);
                monster.Room = this;
            }

            // 타인들에게 입장한 게임 오브젝트 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in players.Values)
                {
                    // 타인이 본인인 경우를 제외
                    if (p.Id != gameObject.Id)
                    {
                        p.Session.Send(spawnPacket);                        
                    }
                }
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (players.Remove(objectId, out player) == false)
                {
                    return;
                }

                player.Room = null;
            }

            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                foreach (Player p in players.Values)
                {
                    p.Session.Send(despawnPacket);
                }
            }

        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
            {
                return;
            }

            player.pendingInputs.Enqueue(movePacket);
        }

        public void HandleMeleeAttack(Player player, C_MeleeAttack attack)
        {
            player.SetMeleeAttack(attack);
        }
    }
}