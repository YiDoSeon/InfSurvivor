using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared;

namespace Server.Game.Job
{
    public struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int execTick;
        public IJob job;
        public int CompareTo(JobTimerElement other)
            => other.execTick - execTick;
    }
    
    public class JobTimer
    {
        private readonly PriorityQueue<JobTimerElement> pq = new PriorityQueue<JobTimerElement>();
        private object _lock = new object();

        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerElement jobTimerElement;
            jobTimerElement.execTick = System.Environment.TickCount + tickAfter;
            jobTimerElement.job = job;

            lock (_lock)
            {
                pq.Push(jobTimerElement);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;
                JobTimerElement jobTimerElement;

                lock (_lock)
                {
                    if (pq.Count == 0)
                    {
                        break;
                    }

                    jobTimerElement = pq.Peek();

                    if (jobTimerElement.execTick > now)
                    {
                        break;
                    }

                    pq.Pop();
                }

                jobTimerElement.job.Execute();
            }
        }
    }
}