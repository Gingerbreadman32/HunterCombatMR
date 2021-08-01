using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Animation
{
    public sealed class AnimationLayers
        : KeyframeDataCollection<Layer, LayerData>
    {
        public AnimationLayers()
            : base()
        { }

        public AnimationLayers(AnimationLayers copy)
            : base(copy)
        { }

        public AnimationLayers(ExtraAnimationData legacy)
        {
            _references = new List<Layer>();
            _frameData = new SortedList<FrameIndex, KeyframeData<LayerData>>();

            foreach (var frame in legacy.KeyFrameProfile)
            {
                _frameData.Add(_frameData.Count, new KeyframeData<LayerData>(frame));
            }

            foreach (var layer in legacy.Layers)
            {
                var newLayer = new Layer(layer);
                _references.Add(newLayer);

                foreach (var frameData in layer.KeyFrames.Where(x => x.Value.IsEnabled))
                {
                    _frameData[frameData.Key].Add(layer.DisplayName, new LayerData(frameData.Value));
                }
            }
        }

        public AnimationLayers(IEnumerable<Layer> references, SortedList<FrameIndex, KeyframeData<LayerData>> frameData)
        {
            _references = new List<Layer>(references);
            _frameData = frameData;
        }

        public LayerReference this[string layer, FrameIndex index]// @@warn
        {
            get => new LayerReference(this[layer], _frameData[index][layer], index, GetLayerDepth(layer, index));
            set { { this[layer] = value.Layer; _frameData[index][layer] = value.FrameData; SetLayerDepth(layer, index, value.CurrentDepth); } }
        }

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

        public bool TryGetValue(string name, FrameIndex index, out LayerReference value)// @@warn
        {
            bool result = _frameData[index].TryGetValue(name, out var frameData);
            value = new LayerReference(this[name], frameData, index);
            return result;
        }

        internal void DropDownKeyFrames(FrameIndex keyFrameIndex)// @@warn
        {
            _frameData.Remove(keyFrameIndex);
            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        internal void MoveKeyFrameData(string layer,
                    FrameIndex keyFrameIndex,
            FrameIndex newFrameIndex)// @@warn
        {
            if (!ContainsKey(layer))
                return;

            LayerData temp = _frameData[keyFrameIndex][layer];
            _frameData[keyFrameIndex][layer] = _frameData[newFrameIndex][layer];
            _frameData[newFrameIndex][layer] = temp;
        }

        private void InheritPreviousKeyFrameProperties(FrameIndex keyFrameIndex)// @@warn
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