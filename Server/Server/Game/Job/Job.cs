using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Game.Job
{
    public interface IJob
    {
        void Execute();
    }
    
    public class Job : IJob
    {
        private Action action;

        public Job(Action action)
        {
            Debug.Assert(action != null, "action is null");
            this.action = action;
        }

        public void Execute()
        {
            action.Invoke();
        }
    }

    public class Job<T1> : IJob
    {
        private Action<T1> action;
        private T1 t1;

        public Job(Action<T1> action, T1 t1)
        {
            Debug.Assert(action != null, "action is null");
            this.action = action;
            this.t1 = t1;
        }

        public void Execute()
        {
            action.Invoke(t1);
        }
    }

    public class Job<T1, T2> : IJob
    {
        private Action<T1, T2> action;
        private T1 t1;
        private T2 t2;

        public Job(Action<T1, T2> action, T1 t1, T2 t2)
        {
            Debug.Assert(action != null, "action is null");
            this.action = action;
            this.t1 = t1;
            this.t2 = t2;
        }

        public void Execute()
        {
            action.Invoke(t1, t2);
        }
    }

    public class Job<T1, T2, T3> : IJob
    {
        private Action<T1, T2, T3> action;
        private T1 t1;
        private T2 t2;
        private T3 t3;

        public Job(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            Debug.Assert(action != null, "action is null");
            this.action = action;
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
        }

        public void Execute()
        {
            action.Invoke(t1, t2, t3);
        }
    }
}