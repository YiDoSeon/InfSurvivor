using System;
using System.Collections.Generic;

namespace Server.Game.Job
{
    public class JobSerializer
    {
        private JobTimer timer = new JobTimer();
        private Queue<IJob> jobQueue = new Queue<IJob>();
        private object _lock = new object();

        #region PushAfter Helper            
        public IJob PushAfter(int tickAfter, Action action)
    => PushAfter(tickAfter, new Job(action));
        public IJob PushAfter<T1>(int tickAfter, Action<T1> job, T1 t1)
            => PushAfter(tickAfter, new Job<T1>(job, t1));
        public IJob PushAfter<T1, T2>(int tickAfter, Action<T1, T2> job, T1 t1, T2 t2)
            => PushAfter(tickAfter, new Job<T1, T2>(job, t1, t2));
        public IJob PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> job, T1 t1, T2 t2, T3 t3)
            => PushAfter(tickAfter, new Job<T1, T2, T3>(job, t1, t2, t3));

        public IJob PushAfter(int tickAfter, IJob job)
        {
            timer.Push(job, tickAfter);
            return job;
        }
        #endregion

        #region Push Helper            
        public void Push(Action action)
            => Push(new Job(action));
        public void Push<T1>(Action<T1> action, T1 t1)
            => Push(new Job<T1>(action, t1));
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
            => Push(new Job<T1, T2>(action, t1, t2));
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
            => Push(new Job<T1, T2, T3>(action, t1, t2, t3));
        #endregion

        public void Push(IJob job)
        {
            lock (_lock)
            {
                jobQueue.Enqueue(job);
            }
        }

        protected void Flush()
        {
            timer.Flush();

            while (true)
            {
                IJob job = Pop();
                if (job == null)
                {
                    return;
                }

                job.Execute();
            }
        }

        private IJob Pop()
        {
            lock (_lock)
            {
                if (jobQueue.Count == 0)
                {
                    return null;
                }
                return jobQueue.Dequeue();
            }
        }
    }
}