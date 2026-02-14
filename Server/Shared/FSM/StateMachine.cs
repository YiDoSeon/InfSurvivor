using System.Collections.Generic;

namespace Shared.FSM
{
    public abstract class StateMachine
    {
        public abstract void Update();
        public abstract void FixedUpdate();
    }
    
    public class StateMachine<TEntity, TEnum> : StateMachine where TEnum : System.Enum
    {
        private TEntity entity;
        public TEntity Entity => entity;
        private Dictionary<TEnum, FiniteState<TEntity, TEnum>> stateCache = new Dictionary<TEnum, FiniteState<TEntity, TEnum>>();
        public FiniteState<TEntity, TEnum> CurrentState { get; private set; }
        public TEnum CurrentStateId { get; private set; }

        public StateMachine(TEntity entity)
        {
            this.entity = entity;
        }

        public void AddState(TEnum id, FiniteState<TEntity, TEnum> state)
        {
            stateCache[id] = state;
        }

        public void Initialize(TEnum initialId)
        {
            CurrentStateId = initialId;
            CurrentState = stateCache[initialId];
            CurrentState?.Enter();
        }

        public void ChangeState(TEnum newId)
        {
            if (EqualityComparer<TEnum>.Default.Equals(CurrentStateId, newId))
            {
                return;
            }
            CurrentState?.Exit();
            CurrentStateId = newId;
            CurrentState = stateCache[newId];
            CurrentState?.Enter();
        }

        public override void Update()
        {
            CurrentState?.Update();
        }

        public override void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }
    }
}
