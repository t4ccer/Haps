using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using t4ccer.Haps.Collections;

namespace t4ccer.Haps
{
    public abstract class DataProducer<T>
    {
        public delegate void DataProducedHandler(object sender, T item);
        public event DataProducedHandler OnDataProduced;
        public delegate void DataConsumedHandler(object sender, T item);
        public event DataConsumedHandler OnDataConsumed;


        public int PregeneratedDataQueueSize { get; private set; }
        private readonly Queue<T> Pregenerated = new Queue<T>();

        public DataProducer(int pregeneratedDataQueueSize)
        {
            PregeneratedDataQueueSize = pregeneratedDataQueueSize;
            Pregenerated.OnDequeue += RegenerateDataHandler;
            Task.Run(() => Parallel.For(0, PregeneratedDataQueueSize, _ => RegenerateData()));
        }

        private void RegenerateDataHandler(object sender, T item)
            => RegenerateData();
        private void RegenerateData()
        {
            var data = GenerateData();
            Pregenerated.Enqueue(data);
            OnDataProduced?.Invoke(this, data);
        }

        public T ConsumeData()
        {
            while (true)
            {
                var succ = Pregenerated.TryDequeue(out var data);
                if (succ)
                {
                    OnDataConsumed?.Invoke(this, data);
                    return data;
                }
            }
        }

        public abstract T GenerateData();
    }
}
