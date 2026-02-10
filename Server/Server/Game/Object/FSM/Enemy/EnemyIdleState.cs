using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.FSM;
using Shared.Packet;

namespace Server.Game.Object.FSM.Enemy
{
    public class EnemyIdleState : EnemyStateBase
    {
        public EnemyIdleState(Object.Enemy entity, StateMachine<Object.Enemy, EnemyState> stateMachine) : base(entity, stateMachine)
        {
        }
    }
}