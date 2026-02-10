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
        private float knockBackSpeed = 5f;
        public Enemy()
        {
            ObjectType = GameObjectType.Monster;
            CreateStateMachine();
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

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            stateMachine.FixedUpdate();
        }

        public void OnDamaged(GameObject sender)
        {
            if (sender is Player player)
            {
                KnockBackDir = player.FacingDir;
                //Console.WriteLine(KnockBackDir);
            }
            //Console.WriteLine(Info.Name);
            stateMachine.ChangeState(EnemyState.Damaged);
        }

        public void KnockBack()
        {
            Pos += KnockBackDir.normalized * knockBackSpeed * GameLogic.FIXED_DELTA_TIME;
        }

        public void OnCustomTriggerEnter(ColliderBase other)
        {
            // if (other.Owner is Player player)
            // {
            //     Console.WriteLine(player.Info.Name);
            // }
        }

        public void OnCustomTriggerExit(ColliderBase other)
        {
            
        }

        public void OnCustomTriggerStay(ColliderBase other)
        {
            
        }
    }
}