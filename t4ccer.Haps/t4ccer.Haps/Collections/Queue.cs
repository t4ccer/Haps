namespace t4ccer.Haps.Collections
{
    public class Queue<T> : System.Collections.Concurrent.ConcurrentQueue<T>
    {
        public delegate void EnqueueHandler(object sender, T item);
        public event EnqueueHandler OnEnqueue;
        public delegate void DequeueHandler(object sender, T item);
        public event DequeueHandler OnDequeue;

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnEnqueue?.Invoke(this, item);
        }
        public new bool TryDequeue(out T result)
        {
            bool succ = base.TryDequeue(out T item);
            result = item;
            if (succ)
                OnDequeue?.Invoke(this, item);
            return succ;
        }
    }
}
