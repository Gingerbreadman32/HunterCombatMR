using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models
{
    public class KeyframeData<T>
        : IDictionary<string, T>, 
        IKeyframeData
    {
        private IDictionary<string, T> _data;

        public KeyframeData(FrameLength frames)
        {
            Frames = frames;
            _data = new Dictionary<string, T>();
        }

        
        public KeyframeData(FrameLength frames,
            IDictionary<string, T> data)
        {
            Frames = frames;
            _data = new Dictionary<string, T>(data);
        }

        public KeyframeData(FrameLength frames,
            string reference,
            T data)
        {
            Frames = frames;
            _data = new Dictionary<string, T>() { { reference, data } };
        }

        [JsonConstructor]
        public KeyframeData(IEnumerable<KeyValuePair<string, T>> data)
        {
            Frames = FrameLength.One;
            _data = data.ToDictionary(x => x.Key, x => x.Value);
        }

        public int Count => _data.Count;

        public FrameLength Frames { get; set; }

        public bool IsReadOnly { get; set; }

        public ICollection<string> Keys => _data.Keys;

        public ICollection<T> Values => _data.Values;

        public T this[string name]
        {
            get => _data[name];
            set => _data[name] = value;
        }

        public void Add(string key, T value)
        {
            if (_data.ContainsKey(key))
            {
                _data[key] = value;
                return;
            }

            _data.Add(key, value);
        }

        public void Add(KeyValuePair<string, T> item)
        {
            _data.Add(item);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
            => _data.Contains(item);

        public bool ContainsKey(string key)
            => _data.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (var data in _data)
            {
                yield return new KeyValuePair<string, T>(data.Key, data.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool Remove(string key)
            =>_data.Remove(key);


        public bool Remove(KeyValuePair<string, T> item)
            => _data.Remove(item);

        public bool TryGetValue(string key, out T value)
        {
            if (!ContainsKey(key))
            {
                value = default(T);
                return false;
            }

            value = this[key];
            return true;
        }
    }
}