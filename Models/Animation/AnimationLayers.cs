using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Animation
{
    public sealed class AnimationLayers
        : IDictionary<string, Layer>
    {
        public SortedList<FrameIndex, KeyFrameData<LayerData>> FrameData { get; set; }
        private ICollection<Layer> _layers;

        public AnimationLayers()
        {
            _layers = new List<Layer>();
            FrameData = new SortedList<FrameIndex, KeyFrameData<LayerData>>() { { 0, new KeyFrameData<LayerData>(1) } };
        }

        public AnimationLayers(AnimationLayers copy)
        {
        }

        public AnimationLayers(ExtraAnimationData legacy)
        {
        }

        public int Count => _layers.Count;
        public bool IsReadOnly { get; set; }
        public ICollection<string> Keys => _layers.Select(x => x.DisplayName).ToList();

        public ICollection<Layer> Values { get => _layers; }

        public Layer this[string layer]
        {
            get => Values.SingleOrDefault(x => x.DisplayName.Equals(layer));
            set { if (Keys.Contains(layer)) { _layers.Remove(_layers.Single(x => x.DisplayName.Equals(layer))); _layers.Add(value); } }
        }

        public (Layer layer, LayerData frameData) this[string layer, FrameIndex index]
        {
            get => new ValueTuple<Layer, LayerData>(this[layer], FrameData[index][layer]);
            set { { this[layer] = value.layer; FrameData[index][layer] = value.frameData; } }
        }

        public void Add(Layer layer)
        {
            this[layer.DisplayName] = layer;
        }

        public void Add(string key, Layer value)
        {
            var newLayer = new Layer(value);
            newLayer.DisplayName = key;
            this[key] = newLayer;
        }

        public void Add(KeyValuePair<string, Layer> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _layers.Clear();
        }

        public bool Contains(KeyValuePair<string, Layer> item)
            => ContainsKey(item.Key) && _layers.Contains(item.Value);

        public bool ContainsKey(string key)
            => Keys.Contains(key);

        public void CopyTo(KeyValuePair<string, Layer>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, Layer>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => Values.GetEnumerator();

        public IEnumerable<Layer> GetLayersByTag(TextureTag tag)
            => Values.Where(x => x.Tag.Equals(tag));
        /*
        public IOrderedEnumerable<(Layer layer, LayerData frameData)> GetOrderedActiveLayerData(FrameIndex keyFrame)
                                                                                                    => _layers
                .Where(x => x.IsActive(keyFrame))
                .Select(x => new KeyValuePair<Layer, LayerData>(x.Value, x.Value.KeyFrameData[keyFrame]))
                .OrderByDescending(x => x.Key);
        */
        public bool Remove(string key)
        {
            if (Keys.Contains(key))
            {
                _layers.Remove(_layers.Single(x => x.DisplayName.Equals(key)));
                return !Keys.Contains(key);
            }

            return false;
        }

        public bool Remove(KeyValuePair<string, Layer> item)
        {
            if (!Contains(item))
                return false;

            Remove(item.Key);
            return true;
        }

        public bool TryGetValue(string key, out Layer value)
        {
            value = null;

            if (!ContainsKey(key))
                return false;

            value = this[key];

            return value != null;
        }

        internal void MoveKeyFrameData(string layer,
            FrameIndex keyFrameIndex,
            FrameIndex newFrameIndex)
        {
            if (!ContainsKey(layer))
                return;

            LayerData temp = FrameData[keyFrameIndex][layer];
            FrameData[keyFrameIndex][layer] = FrameData[newFrameIndex][layer];
            FrameData[newFrameIndex][layer] = temp;
        }

        internal void RemoveKeyFrameData(FrameIndex keyFrameIndex)
        {
            FrameData.Remove(keyFrameIndex);
            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        private void InheritPreviousKeyFrameProperties(FrameIndex keyFrameIndex)
        {
            int nextFrameIndex = keyFrameIndex + 1;
            if (FrameData.ContainsKey(nextFrameIndex))
            {
                FrameData.Add(keyFrameIndex, FrameData[nextFrameIndex]);
                FrameData.Remove(nextFrameIndex);
                InheritPreviousKeyFrameProperties(nextFrameIndex);
            }
        }
    }
}