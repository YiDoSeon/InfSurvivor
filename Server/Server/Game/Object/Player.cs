using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Session;
using Shared.Packet;

namespace Server.Game.Object
{
    public class Player : GameObject
    {
        public ClientSession Session { get; set; }
        public Queue<IPacket> pendingInputs = new Queue<IPacket>();
        public uint LastProcessedSequence { get; set; }
        private long lastClientTime;

        public void Move(float deltaTime, long serverTime)
        {
            bool needToSync = false;
            float previousVelocityY = Velocity.y; // 이전 프레임 속도 저장

            while (pendingInputs.Count > 0)
            {
                //Console.WriteLine($"#({LastProcessedSequence}) ({Pos}) ({Velocity})");
                IPacket packet = pendingInputs.Dequeue();
                if (packet is C_Move movePacket)
                {
                    PositionInfo info = movePacket.PosInfo;
                    Pos = info.Pos; // 이동 방향 의해 변경될 대상 좌표
                    Velocity = info.Velocity; // 이동 방향 및 속도
                    //Console.WriteLine($"#{movePacket.SeqNumber} ({Velocity})");

                    LastProcessedSequence = movePacket.SeqNumber;
                    lastClientTime = movePacket.ClientTime;
                }
                needToSync = true;
            }

            // 공중에 있거나, 좌우 이동속도가 있으면 물리 처리 진행
            bool needsMovement = Velocity.x != 0f || Velocity.y != 0f;

            if (needsMovement)
            {
                Pos += Velocity * deltaTime;
            }

            if (needToSync)
            {
                SendMove();
            }
        }

        private void SendMove()
        {
            S_Move move = new S_Move
            {
                ObjectId = Id,
                SeqNumber = LastProcessedSequence,
                ClientTime = lastClientTime,
                PosInfo = new PositionInfo
                {
                    Pos = Pos,
                    Velocity = Velocity
                }
            };
            //Console.WriteLine($"#{move.SeqNumber} ({Pos}) ({Velocity})");
            BroadCast(move);
        }
        
        private void BroadCast<T>(T packet) where T: IPacket
        {
            List<ClientSession> sessions = SessionManager.Instance.GetSessions();
            foreach (ClientSession session in sessions)
            {
                session.Send(packet);
            }
        }
    }
}