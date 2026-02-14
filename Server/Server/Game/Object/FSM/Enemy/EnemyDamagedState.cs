using System;
using Server.Game.Room;
using Shared.FSM;
using Shared.Packet;
using Shared.Packet.Struct;

namespace Server.Game.Object.FSM.Enemy
{
    public class EnemyDamagedState : EnemyStateBase
    {
        public EnemyDamagedState(Object.Enemy entity, StateMachine<Object.Enemy, EnemyState> stateMachine) : base(entity, stateMachine)
        {
        }

        public override void Enter()
        {
        }

        public override void FixedUpdate()
        {
            if ((entity.Pos - entity.ExpectedPos).sqrMagnitude < 0.01f)
            {
                stateMachine.ChangeState(EnemyState.Idle);
            }
        }

        public override void Exit()
        {
            entity.ResetKnockBackDir();
            Console.WriteLine($"실제 {entity.Info.Name}: {entity.Pos}");
        }
    }
}