using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.FSM;
using Shared.Packet;

namespace Server.Game.Object.FSM.Enemy
{
    using Enemy = Server.Game.Object.Enemy;
    public class EnemyStateBase : FiniteState<Enemy, EnemyState>
    {
        public EnemyStateBase(Enemy entity, StateMachine<Enemy, EnemyState> stateMachine) : base(entity, stateMachine)
        {
        }

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            
        }

        public override void FixedUpdate()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}