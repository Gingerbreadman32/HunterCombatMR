using HunterCombatMR.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models
{
    public class KeyframeDataCollection<TReference, TData>
        : IDictionary<string, TReference>
        where TData : class
        where TReference : IKeyframeDataReference
    {
        private protected KeyframeData<TData>[] _frameData;

        private protected ICollection<TReference> _references;

        public KeyframeDataCollection()
        {
            _references = new List<TReference>();
            _frameData = new KeyframeData<TData>[0];
        }

        public KeyframeDataCollection(KeyframeDataCollection<TReference, TData> copy)
        {
            _references = copy._references;
            _frameData = copy._frameData;
        }

        public int Count => _references.Count;

        public KeyframeData<TData>[] FrameData
        {
            get => _frameData;
        }

        public bool IsReadOnly { get; set; }
        public ICollection<string> Keys => _references.Select(x => x.Name).ToList();

        public ICollection<TReference> Values { get => _references; }

        public TReference this[string referenceName]
        {
            get => Values.SingleOrDefault(x => x.Name.Equals(referenceName));
            set { if (Keys.Contains(referenceName)) { _references.Remove(_references.Single(x => x.Name.Equals(referenceName))); _references.Add(value); } }
        }

        public KeyframeData<TData> this[FrameIndex index]
        {
            get => _frameData[index];
            set => _frameData[index] = value;
        }

        public void Add(TReference layer)
        {
            this[layer.Name] = layer;
        }

        public void Add(string key, TReference value)
        {
            this[key] = value;
        }

        public void Add(KeyValuePair<string, TReference> item)
        {
            Add(item.Key, item.Value);
        }

        public virtual void AddToKeyframe(FrameIndex keyframe,
            string referenceName,
            TData data)
        {
            if (this[keyframe] == null)
                throw new Exception($"No keyframe {keyframe} exists for current data collection.");

            if (this[referenceName] == null)
                throw new Exception($"No data reference {referenceName} exists for current data collection.");

            if (this[keyframe].ContainsKey(referenceName))
            {
                this[keyframe][referenceName] = data;
                return;
            }

            this[keyframe].Add(referenceName, data);
        }

        public void Clear()
        {
            _references.Clear();
        }

        public bool Contains(KeyValuePair<string, TReference> item)
            => ContainsKey(item.Key) && _references.Contains(item.Value);

        public bool ContainsKey(string key)
            => Keys.Contains(key);

        public void CopyTo(KeyValuePair<string, TReference>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, TReference>> GetEnumerator()
        {
            foreach (var reference in _references)
            {
                yield return new KeyValuePair<string, TReference>(reference.Name, reference);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool Remove(string key)
        {
            if (Keys.Contains(key))
            {
                _references.Remove(_references.Single(x => x.Name.Equals(key)));
                return !Keys.Contains(key);
            }

            return false;
        }

        public bool Remove(KeyValuePair<string, TReference> item)
        {
            if (!Contains(item))
                return false;

            Remove(item.Key);
            return true;
        }

        public bool TryGetValue(string key, out TReference value)
        {
            value = default(TReference);

            if (!ContainsKey(key))
                return false;

            value = this[key];

            return value != null;
        }
    }
}