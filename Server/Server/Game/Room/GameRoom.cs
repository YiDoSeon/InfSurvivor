using System;
using System.Collections.Generic;
using System.Diagnostics;
using Server.Game.Job;
using Server.Game.Object;
using Shared.Packet;
using Shared.Packet.Struct;
using Shared.Physics;

namespace Server.Game.Room
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();
        public long LastServerTime { get; private set; }
        public CollisionWorld CollisionWorld { get; private set; } = new CollisionWorld();

        public void Init()
        {
            Random rand = new Random();
            float min = -100f;
            float max = 100f;
            for (int i = 0; i < 1000; i++)
            {
                Monster monster = ObjectManager.Instance.Add<Monster>();
                {
                    monster.Info.Name = $"Monster-{i}";
                    monster.SetRoom(this);
                    monster.InitPos(new PositionInfo()
                    {
                        Pos = new CVector2(
                            rand.NextSingle() * (max - min) + min,
                            rand.NextSingle() * (max - min) + min)
                    });
                    EnterGame(monster);
                }
            }

            CollisionWorld.Init();
        }

        public void Update(float deltaTime, long serverTime)
        {
            LastServerTime = serverTime;
            Flush();

            //Stopwatch sww = new Stopwatch();
            foreach (Player player in players.Values)
            {
                player.OnTick(deltaTime);
            }
            foreach (Monster monster in monsters.Values)
            {
                monster.OnTick(deltaTime);
            }

            CollisionWorld.OnTick();
            //Console.WriteLine(sww.Elapsed.TotalMicroseconds);
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
                    foreach (Monster m in monsters.Values)
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
                Monster monster = (Monster)gameObject;
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
    }
}