using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared
{
    public class PriorityQueue<T>
    {
        private readonly List<T> heap = new List<T>();
        private readonly IComparer<T> comparer;

        public PriorityQueue(IComparer<T> comparer = null)
        {
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        /// <summary>
        /// index1이 index2보다 클 때만 true를 반환 
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        private bool Higher(int index1, int index2)
            => comparer.Compare(heap[index1], heap[index2]) > 0;

        public int Count => heap.Count;

        public void Push(T data)
        {
            heap.Add(data);

            int now = heap.Count - 1;

            while (now > 0)
            {
                int next = (now - 1) / 2;

                if (Higher(now, next) == false)
                {
                    break;
                }

                T temp = heap[now];
                heap[now] = heap[next];
                heap[next] = temp;

                now = next;
            }
        }

        public T Pop()
        {
            T ret = heap[0];

            int lastIndex = heap.Count - 1;
            heap[0] = heap[lastIndex];
            heap.RemoveAt(lastIndex);
            lastIndex--;

            int now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;

                if (left <= lastIndex && Higher(next, left) == false)
                {
                    next = left;
                }
                if (right <= lastIndex && Higher(next,right) == false)
                {
                    next = right;
                }

                if (next == now)
                {
                    break;
                }

                T temp = heap[next];
                heap[next] = heap[now];
                heap[now] = temp;

                now = next;
            }

            return ret;
        }

        public T Peek()
        {
            if (heap.Count == 0)
            {
                return default;
            }
            return heap[0];
        }
    }
}