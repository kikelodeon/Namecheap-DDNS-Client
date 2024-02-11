using System.Collections;
namespace eggDDNS
{
    class CircularBuffer<T> : IEnumerable<T>
    {
        private T[] buffer;
        private int head;
        private int tail;
        private int size;
        private bool full;

        public CircularBuffer(int capacity)
        {
            buffer = new T[capacity];
            size = capacity;
            head = 0;
            tail = 0;
            full = false;
            Count = 0;
        }

        public int Count { get; private set; }

        public void Enqueue(T item)
        {
            buffer[head] = item;
            head = (head + 1) % size;
            if (full)
                tail = (tail + 1) % size;
            else if (head == tail)
                full = true;
            else
                Count++;
        }

        public T Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("Buffer is empty.");

            T dequeuedItem = buffer[tail];
            tail = (tail + 1) % size;
            Count--;

            return dequeuedItem;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return buffer[(tail + i) % size];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
