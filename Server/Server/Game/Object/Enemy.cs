using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Game.Object.FSM.Enemy;
using Server.Game.Room;
using Shared.FSM;
using Shared.Packet;
using Shared.Packet.Struct;
using Shared.Physics.Collider;

namespace Server.Game.Object
{
    public class Enemy : GameObject, IColliderTrigger
    {
        private CCircleCollider bodyCollider;
        public override ColliderBase BodyCollider => bodyCollider;
        private StateMachine<Enemy, EnemyState> stateMachine;
        public CVector2 KnockBackDir { get; private set; }
        public float KnockBackSpeed { get; private set; } = 10f;
        public float KnockBackTime { get; private set; } = 0.1f;
        public CVector2 ExpectedPos { get; private set; }
        public Enemy()
        {
            ObjectType = GameObjectType.Monster;
            CreateStateMachine();
        }

        public override void InitPos(PositionInfo posInfo)
        {
            base.InitPos(posInfo);
            ExpectedPos = Pos;
        }

        private void CreateStateMachine()
        {
            stateMachine = new StateMachine<Enemy, EnemyState>(this);
            stateMachine.AddState(EnemyState.Idle, new EnemyIdleState(this, stateMachine));
            stateMachine.AddState(EnemyState.Move, new EnemyMoveState(this, stateMachine));
            stateMachine.AddState(EnemyState.Damaged, new EnemyDamagedState(this, stateMachine));
            stateMachine.Initialize(EnemyState.Idle);
        }

        public override void SetRoom(GameRoom room)
        {
            base.SetRoom(room);
            bodyCollider = new CCircleCollider(
                this,
                CVector2.zero,
                Pos,
                0.5f);
            bodyCollider.Layer = CollisionLayer.Monster;
            Room.CollisionWorld.RegisterCollider(bodyCollider);
        }

        public override void OnTick()
        {
            base.OnTick();
            float speed = 0f;
            if (stateMachine.CurrentStateId == EnemyState.Damaged)
            {
                speed = KnockBackSpeed;
            }
            else if (stateMachine.CurrentStateId == EnemyState.Move)
            {
                speed = MoveSpeed;
            }
            Pos = CVector2.MoveTowards(Pos, ExpectedPos, speed * GameLogic.FIXED_DELTA_TIME);
            stateMachine.FixedUpdate();
        }

        public void OnDamaged(GameObject sender)
        {
            if (sender is Player player)
            {
                KnockBackDir += player.FacingDir;
            }
            ExpectedPos = Pos + KnockBackDir.normalized * KnockBackSpeed * KnockBackTime;
            stateMachine.ChangeState(EnemyState.Damaged);
        }

        public void ResetKnockBackDir()
        {
            KnockBackDir = CVector2.zero;
        }

        public void OnCustomTriggerEnter(ColliderBase other)
        {
        }

        public void OnCustomTriggerExit(ColliderBase other)
        {
            
        }

        public void OnCustomTriggerStay(ColliderBase other)
        {
            
        }
    }
}