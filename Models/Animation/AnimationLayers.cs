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
            var frameData = new List<KeyframeData<LayerData>>();

            foreach (var frame in legacy.KeyFrameProfile)
            {
                frameData.Add(new KeyframeData<LayerData>(frame));
            }

            foreach (var layer in legacy.Layers)
            {
                var newLayer = new Layer(layer);
                _references.Add(newLayer);

                foreach (var data in layer.KeyFrames.Where(x => x.Value.IsEnabled))
                {
                    frameData[data.Key].Add(layer.DisplayName, new LayerData(data.Value));
                }
            }

            _frameData = frameData.ToArray();
        }

        public AnimationLayers(IEnumerable<Layer> references, KeyframeData<LayerData>[] frameData)
        {
            _references = new List<Layer>(references);
            _frameData = frameData;
        }

        public LayerReference this[string layer, FrameIndex index]
        {
            get => new LayerReference(this[layer], _frameData[index][layer], index, GetLayerDepth(layer, index));
            private set { { this[layer] = value.Layer; _frameData[index][layer] = value.FrameData; } }
        }

        public int GetLayerDepth(string layer, FrameIndex index)
        {
            if (_frameData[index][layer].DepthOverride.HasValue)
                return _frameData[index][layer].DepthOverride.Value;

            return this[layer].Depth;
        }

        public IEnumerable<Layer> GetLayersByTag(TextureTag tag)
            => Values.Where(x => x.Tag.Equals(tag));

        public IOrderedEnumerable<LayerReference> GetOrderedActiveLayerData(FrameIndex keyframe)
            => _frameData[keyframe]
                .Select(x => new LayerReference(this[x.Key], x.Value, keyframe))
                .OrderByDescending(x => x.CurrentDepth);

        public bool TryGetValue(string name, FrameIndex index, out LayerReference value)
        {
            bool result = _frameData[index].TryGetValue(name, out var frameData);
            value = new LayerReference(this[name], frameData, index, GetLayerDepth(name, index));
            return result;
        }
    }
}