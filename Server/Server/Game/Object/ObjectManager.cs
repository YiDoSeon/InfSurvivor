using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Packet;

namespace Server.Game.Object
{
    public class ObjectManager
    {
        private static ObjectManager instance = new ObjectManager();
        public static ObjectManager Instance => instance;

        private object _lock = new object();
        private Dictionary<int, Player> players = new Dictionary<int, Player>();
        private int counter = 0;

        public T Add<T>() where T : GameObject, new()
        {
            T gameObject = new T();

            lock (_lock)
            {
                gameObject.Id = GenerateId(gameObject.ObjectType);

                if (gameObject.ObjectType == GameObjectType.Player)
                {
                    players.Add(gameObject.Id, gameObject as Player);
                }
            }
            
            return gameObject;
        }

        private int GenerateId(GameObjectType type)
        {
            // type은 8비트로만 표현 (실제로는 7bit), 나머지는 id
            return ((int)type << 24) | counter++;
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            // type 비트를 가져와서 7bit로 제한 (7bit을 넘어갔을 경우 대비)
            int type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }

        public bool Remove(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    return players.Remove(objectId);
                }
            }

            return false;
        }

        public Player Find(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    if (players.TryGetValue(objectId, out Player player))
                    {
                        return player;
                    }
                }
                return null;
            }
        }
    }
}