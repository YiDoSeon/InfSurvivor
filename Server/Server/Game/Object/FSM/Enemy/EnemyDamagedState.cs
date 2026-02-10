using System;
using Server.Game.Room;
using Shared.FSM;
using Shared.Packet;

namespace Server.Game.Object.FSM.Enemy
{
    public class EnemyDamagedState : EnemyStateBase
    {
        private float time;
        public EnemyDamagedState(Object.Enemy entity, StateMachine<Object.Enemy, EnemyState> stateMachine) : base(entity, stateMachine)
        {
        }

        public override void Enter()
        {
            time = 0f;
        }

        public override void FixedUpdate()
        {
            if (time >= 0.1f)
            {
                stateMachine.ChangeState(EnemyState.Idle);
            }
            else
            {
                entity.KnockBack();
                time += GameLogic.FIXED_DELTA_TIME;
            }
        }

        public override void Exit()
        {
            //Console.WriteLine($"{entity.Info.Name}: {entity.Pos}");
        }
    }
}