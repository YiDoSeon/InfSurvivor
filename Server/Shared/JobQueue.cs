using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared
{
    public interface IJobQueue
    {
        void Push(Action job);
    }
    
    public class JobQueue : IJobQueue
    {
        private Queue<Action> jobQueue = new Queue<Action>();
        private object _lock = new object();
        private bool flushing = false;

        public void Push(Action job)
        {
            bool canFlush = false;

            lock (_lock)
            {
                jobQueue.Enqueue(job);
                if (flushing == false)
                {
                    flushing = canFlush = true;
                }
            }

            if (canFlush)
            {
                Flush();
            }
        }

        private Action Pop()
        {
            // Push에서도 jobQueue가 사용되고 있으므로 lock
            lock (_lock)
            {
                if (jobQueue.Count == 0)
                {
                    flushing = false;
                    return null;
                }
                return jobQueue.Dequeue();
            }
        }

        private void Flush()
        {
            while (true)
            {
                Action action = Pop();
                if (action == null)
                {
                    return;
                }

                action.Invoke();
            }
        }
    }
}