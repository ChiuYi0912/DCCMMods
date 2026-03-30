using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.haxe.ds;

namespace CoreLibrary.Extensions.Customs
{
    public class IntMapLiveView<T>
    {
        private readonly IntMap _map;
        private readonly Func<dynamic, T> _converter;

        public IntMapLiveView(IntMap map, Func<dynamic, T> converter)
        {
            _map = map;
            _converter = converter;
        }

        public T this[int key]
        {
            get
            {
                if (!_map.exists(key))
                    throw new KeyNotFoundException($"The key '{key}' was not found.");
                return _converter(_map.get(key));
            }
            set => _map.set(key, value);
        }

        public T set(int key, T value)
        {
            _map.set(key, value);
            return value;
        }

        public bool TryGetValue(int key, out T value)
        {
            if (!_map.exists(key))
            {
                value = default!;
                return false;
            }
            value = _converter(_map.get(key));
            return true;
        }

        public bool ContainsKey(int key) => _map.exists(key);
        public bool Remove(int key) => _map.remove(key);
        public void Clear() => _map.clear();

        public int Count
        {
            get
            {
                var keys = _map.keys();
                int count = 0;
                while (keys.hasNext())
                {
                    keys.next();
                    count++;
                }
                return count;
            }
        }

        public IEnumerable<int> Keys
        {
            get
            {
                var keys = _map.keys();
                while (keys.hasNext())
                {
                    yield return keys.next();
                }
            }
        }

        public IEnumerable<T> Values
        {
            get
            {
                var keys = _map.keys();
                while (keys.hasNext())
                {
                    var key = keys.next();
                    yield return _converter(_map.get(key));
                }
            }
        }

        public IEnumerable<KeyValuePair<int, T>> AsEnumerable()
        {
            var keys = _map.keys();
            while (keys.hasNext())
            {
                var key = keys.next();
                yield return new KeyValuePair<int, T>(key, _converter(_map.get(key)));
            }
        }

        public override string ToString()
        {
            return $"IntMapLiveView with {Count} entries";
        }
    }
}