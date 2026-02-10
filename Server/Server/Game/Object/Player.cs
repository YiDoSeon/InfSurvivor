using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Game.Room;
using Server.Session;
using Shared.Packet;
using Shared.Packet.Struct;
using Shared.Physics.Collider;
using Shared.Utils;

namespace Server.Game.Object
{
    public class Player : GameObject, IColliderTrigger
    {
        public ClientSession Session { get; set; }
        public Queue<IPacket> pendingInputs = new Queue<IPacket>();
        private CBoxCollider bodyCollider;
        private CCircleCollider meleeAttackCollider;
        public override ColliderBase BodyCollider => bodyCollider;
        public uint LastProcessedSequence { get; set; }
        private long lastClientTime;
        private bool firePressed;
        public bool ShouldCheckAttack { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public override void SetRoom(GameRoom room)
        {
            base.SetRoom(room);
            bodyCollider = new CBoxCollider(
                this,
                new CVector2(0f, 0.5f),
                Pos,
                new CVector2(0.6f, 1f)
            );
            bodyCollider.Layer = CollisionLayer.Player;
            Room.CollisionWorld.RegisterCollider(bodyCollider);

            meleeAttackCollider = new CCircleCollider(
                this,
                new CVector2(0f, 0.5f),
                Pos,
                0.75f);
            Room.CollisionWorld.RegisterCollider(meleeAttackCollider);
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            bool needToSync = false;

            while (pendingInputs.Count > 0)
            {
                //Console.WriteLine($"#({LastProcessedSequence}) ({Pos}) ({Velocity})");
                IPacket packet = pendingInputs.Dequeue();
                if (packet is C_Move movePacket)
                {
                    PositionInfo info = movePacket.PosInfo;
                    Pos = info.Pos; // 이동 방향 의해 변경될 대상 좌표
                    Velocity = info.Velocity; // 이동 방향 및 속도
                    FacingDir = info.FacingDir;
                    firePressed = info.FirePressed; // TODO: firePressed 데이터 위치 수정
                    //Console.WriteLine($"#{movePacket.SeqNumber} ({Velocity})");

                    LastProcessedSequence = movePacket.SeqNumber;
                    lastClientTime = movePacket.ClientTime;
                }
                needToSync = true;
            }

            bool needsMovement = Velocity.x != 0f || Velocity.y != 0f;

            if (needsMovement)
            {
                Pos += Velocity * deltaTime;
            }

            if (ShouldCheckAttack)
            {
                CVector2 dir4 = CVector2.zero;
                float absX = MathF.Abs(FacingDir.x);
                float absY = MathF.Abs(FacingDir.y);
                if (absX >= absY)
                {
                    dir4.x = MathF.Sign(FacingDir.x);
                    dir4.y = 0f;
                }
                else
                {
                    dir4.x = 0f;
                    dir4.y = MathF.Sign(FacingDir.y);                    
                }
                
                if (dir4 != CVector2.zero)
                {
                    meleeAttackCollider.Position = Pos + dir4 * 0.8f;
                    List<ColliderBase> colliders = Room.CollisionWorld.GetOverlappedColliders(
                        meleeAttackCollider,
                        targetMask: Extensions.CombineMask(CollisionLayer.Monster)
                    );
                    foreach (ColliderBase collider in colliders)
                    {
                        if (collider.Owner is Enemy enemy)
                        {
                            enemy.OnDamaged(this);
                        }
                    }
                }
                ShouldCheckAttack = false;
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
                    Velocity = Velocity,
                    FacingDir = FacingDir,
                    FirePressed = firePressed,
                }
            };
            //Console.WriteLine($"#{move.SeqNumber} ({Pos}) ({Velocity})");
            Room.BroadCast(move);
        }

        public void OnCustomTriggerEnter(ColliderBase other)
        {
            
        }

        public void OnCustomTriggerStay(ColliderBase other)
        {
            
        }

        public void OnCustomTriggerExit(ColliderBase other)
        {
            
        }
    }
}