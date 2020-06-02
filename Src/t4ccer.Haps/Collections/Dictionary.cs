using System;
using System.Collections.Generic;

namespace t4ccer.Haps.Collections
{
    public class Dictionary<TKey, TValue> : System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>
    {
        public delegate void AddHandler(object sender, KeyValuePair<TKey, TValue> item);
        public event AddHandler OnAdd;
        public delegate void RemoveHandler(object sender, KeyValuePair<TKey, TValue> item);
        public event RemoveHandler OnRemove;

        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {

                if(!ContainsKey(key))
                    OnAdd.Invoke(this, new KeyValuePair<TKey, TValue>(key, value));
                base[key] = value;
            }
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            var succ = base.TryAdd(key, value);
            if (succ)
                OnAdd(this, new KeyValuePair<TKey, TValue>(key, value));

            return succ;
        }

        public new bool TryRemove(TKey key, out TValue value)
        {
            var succ = base.TryRemove(key, out var v);
            value = v;
            if (succ)
                OnRemove(this, new KeyValuePair<TKey, TValue>(key, value));

            return succ;
        }

        public new TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!ContainsKey(key))
                OnAdd.Invoke(this, new KeyValuePair<TKey, TValue>(key, addValue));
            var res = base.AddOrUpdate(key, addValue, updateValueFactory);
            return res;
        }
        public new TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!ContainsKey(key))
            {
                var val = addValueFactory(key);
                var res = base.AddOrUpdate(key, val, updateValueFactory);
                OnAdd.Invoke(this, new KeyValuePair<TKey, TValue>(key, val));
                return val;
            }
            return base.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }
    }
}
