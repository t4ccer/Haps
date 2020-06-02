using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace t4ccer.Haps
{
    public class Halter<T>
    {
        public delegate void PassHandler(object sender, T item);
        public event PassHandler OnPass;

        public void Pass(T item)
        {
            OnPass.Invoke(this, item);
        }
        public T Get(Func<T, bool> predicate)
        {
            T item = default;
            Mutex m = new Mutex(true);
            void handler(object _, T i)
            {
                if (!predicate(i))
                    return;

                item = i;
                m.ReleaseMutex();
            }
            OnPass += handler;
            m.WaitOne();
            OnPass -= handler;
            return item;
        }
    }
}
