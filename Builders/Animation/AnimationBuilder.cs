using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Animation.Entity;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Builders.Animation
{
    public class AnimationBuilder
    {
        private FrameLength[] _keyframes;
        private LayerBuilder[] _layers;
        private LoopStyle _loop;
        private string _name;
        private AnimationType _type;

        public AnimationBuilder(string name,
            AnimationType type = AnimationType.None,
            int frameLength = 1)
        {
            _layers = new LayerBuilder[0];
            _name = name;
            _type = type;
            _loop = LoopStyle.Once;
            _keyframes = new FrameLength[1] { frameLength };
        }

        public FrameLength[] Keyframes { get => _keyframes; }
        public LoopStyle LoopType { get => _loop; set => _loop = value; }
        public string Name { get => _name; set => _name = value; }

        public void AddKeyframe(FrameLength length,
            CopyKeyframe action = CopyKeyframe.None,
            int setKeyframe = 1)
        {
            if ((_keyframes.Length + 1) > AnimationConstants.MaxKeyframes)
                throw new IndexOutOfRangeException($"Total keyframes exceed max of {AnimationConstants.MaxKeyframes}!");

            ArrayUtils.Add(ref _keyframes, length);

            foreach (var layer in _layers)
            {
                if (layer.Frames < _keyframes.Length && action != CopyKeyframe.None)
                    layer.DuplicateKeyframe(action, setKeyframe);
            }
        }

        public void AddKeyframes(CopyKeyframe action, 
            params FrameLength[] frames)
        {
            if ((frames.Length + _keyframes.Length) > AnimationConstants.MaxKeyframes)
                throw new IndexOutOfRangeException($"Total keyframes exceed max of {AnimationConstants.MaxKeyframes}!");

            foreach (var frame in frames)
            {
                AddKeyframe(frame, action);
            }
        }

        public LayerBuilder AddLayer(LayerBuilder layerBuilder)
        {
            var builder = new LayerBuilder(layerBuilder);
            ArrayUtils.Add(ref _layers, builder);
            return builder;
        }

        public LayerBuilder AddLayer(EntityAnimationLayer layer,
            IEnumerable<EntityAnimationLayerData> layerData)
        {
            var builder = new LayerBuilder(layer, layerData);
            ArrayUtils.Add(ref _layers, builder);
            return builder;
        }

        public EntityAnimation Build()
        {
            var layers = _layers.Select(x => x.Build());
            return new EntityAnimation(_name, _type, layers, _loop, _keyframes);
        }

        public LayerBuilder GetLayer(string layerName)
        {
            var predicate = new Func<LayerBuilder, bool>(x => x.Name.Equals(layerName));
            if (!_layers.Any(predicate))
                throw new Exception($"No layers named {layerName} in current animation!");

            return _layers.Single(predicate);
        }

        public void RemoveLayer(string layerName)
        {
            var layer = _layers.SingleOrDefault(x => x.Name.Equals(layerName));
            var index = Array.IndexOf(_layers, layer);

            if (layer == null || index < 0)
                return;

            ArrayUtils.Remove(ref _layers, index);
        }
    }
}