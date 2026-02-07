using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Game.Job;
using Server.Game.Object;
using Shared.Packet;

namespace Server.Game.Room
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private long lastServerTime;

        public void Init()
        {

        }

        public void Update(float deltaTime, long serverTime)
        {
            lastServerTime = serverTime;
            Flush();

            foreach (Player player in players.Values)
            {
                player.Move(deltaTime, serverTime);
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
                player.Room = this;

                {
                    S_EnterGame enterGamePacket = new S_EnterGame
                    {
                        Player = player.Info
                    };
                    player.Session.Send(enterGamePacket);
                }

                {
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in players.Values)
                    {
                        ObjectInfo info = new ObjectInfo();
                        info.MergeFrom(p.Info);
                        spawnPacket.Objects.Add(info);
                    }
                    player.Session.Send(spawnPacket);
                }
            }

            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in players.Values)
                {
                    p.Session.Send(spawnPacket);
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