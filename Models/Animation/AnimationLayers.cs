using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Animation
{
    public sealed class AnimationLayers
        : IDictionary<string, Layer>
    {
        private SortedList<FrameIndex, KeyFrameData<LayerData>> _frameData;

        private ICollection<Layer> _layers;

        public AnimationLayers()
        {
            _layers = new List<Layer>();
            _frameData = new SortedList<FrameIndex, KeyFrameData<LayerData>>() { { 0, new KeyFrameData<LayerData>(1) } };
        }

        public AnimationLayers(AnimationLayers copy)
        {
            _layers = copy._layers;
            _frameData = copy._frameData;
        }

        public AnimationLayers(ExtraAnimationData legacy)
        {
            _layers = new List<Layer>();
            _frameData = new SortedList<FrameIndex, KeyFrameData<LayerData>>();

            foreach (var frame in legacy.KeyFrameProfile)
            {
                _frameData.Add(_frameData.Count, new KeyFrameData<LayerData>(frame));
            }

            foreach (var layer in legacy.Layers)
            {
                var newLayer = new Layer(layer);
                _layers.Add(newLayer);

                foreach (var frameData in layer.KeyFrames.Where(x => x.Value.IsEnabled))
                {
                    _frameData[frameData.Key].Add(layer.DisplayName, new LayerData(frameData.Value));
                }
            }
        }

        public int Count => _layers.Count;

        public SortedList<FrameIndex, IKeyFrameData> FrameData
        {
            get => new SortedList<FrameIndex, IKeyFrameData>(_frameData.ToDictionary(x => x.Key, x => (IKeyFrameData)x.Value));
        }

        public bool IsReadOnly { get; set; }
        public ICollection<string> Keys => _layers.Select(x => x.Name).ToList();

        public ICollection<Layer> Values { get => _layers; }

        public Layer this[string layer]
        {
            get => Values.SingleOrDefault(x => x.Name.Equals(layer));
            set { if (Keys.Contains(layer)) { _layers.Remove(_layers.Single(x => x.Name.Equals(layer))); _layers.Add(value); } }
        }

        public LayerReference this[string layer, FrameIndex index]
        {
            get => new LayerReference(this[layer], _frameData[index][layer], index, GetLayerDepth(layer, index));
            set { { this[layer] = value.Layer; _frameData[index][layer] = value.FrameData; SetLayerDepth(layer, index, value.CurrentDepth); } }
        }

        public KeyFrameData<LayerData> this[FrameIndex index]
        {
            get => _frameData[index];
            set => _frameData[index] = value;
        }

        public void Add(Layer layer)
        {
            this[layer.Name] = layer;
        }

        public void Add(string key, Layer value)
        {
            var newLayer = new Layer(value);
            newLayer.Name = key;
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
            foreach (var layer in _layers)
            {
                yield return new KeyValuePair<string, Layer>(layer.Name, layer);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int GetLayerDepth(string layer, FrameIndex index)
        {
            return this[layer].Depth;
        }

        public IEnumerable<Layer> GetLayersByTag(TextureTag tag)
            => Values.Where(x => x.Tag.Equals(tag));

        public IOrderedEnumerable<LayerReference> GetOrderedActiveLayerData(FrameIndex keyframe)
            => _frameData[keyframe]
                .Select(x => new LayerReference(this[x.Key], x.Value, keyframe))
                .OrderByDescending(x => x.CurrentDepth);

        public bool Remove(string key)
        {
            if (Keys.Contains(key))
            {
                _layers.Remove(_layers.Single(x => x.Name.Equals(key)));
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

        public bool SetLayerDepth(string layer,
                                            FrameIndex index,
            int newDepth)
        {
            if (!TryGetValue(layer, index, out var layerReference))
                return false;

            if (layerReference.Layer.Depth.Equals(newDepth))
            {
                if (layerReference.FrameData.DepthOverride.HasValue)
                    layerReference.FrameData.DepthOverride = null;

                return true;
            }

            layerReference.FrameData.DepthOverride = newDepth;
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

        public bool TryGetValue(string name, FrameIndex index, out LayerReference value)
        {
            bool result = _frameData[index].TryGetValue(name, out var frameData);
            value = new LayerReference(this[name], frameData, index);
            return result;
        }

        internal void DropDownKeyFrames(FrameIndex keyFrameIndex)
        {
            _frameData.Remove(keyFrameIndex);
            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        internal void MoveKeyFrameData(string layer,
                    FrameIndex keyFrameIndex,
            FrameIndex newFrameIndex)
        {
            if (!ContainsKey(layer))
                return;

            LayerData temp = _frameData[keyFrameIndex][layer];
            _frameData[keyFrameIndex][layer] = _frameData[newFrameIndex][layer];
            _frameData[newFrameIndex][layer] = temp;
        }

        private void InheritPreviousKeyFrameProperties(FrameIndex keyFrameIndex)
        {
            int nextFrameIndex = keyFrameIndex + 1;
            if (_frameData.ContainsKey(nextFrameIndex))
            {
                _frameData.Add(keyFrameIndex, _frameData[nextFrameIndex]);
                _frameData.Remove(nextFrameIndex);
                InheritPreviousKeyFrameProperties(nextFrameIndex);
            }
        }
    }
}