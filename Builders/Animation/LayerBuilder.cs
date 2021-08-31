using HunterCombatMR.Enumerations;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Builders.Animation
{
    public static class LayerBuilderChainMethods
    {
        public static LayerBuilder WithKeyframe(this LayerBuilder builder,
            int sheetNumber = 0,
            int sheetFrame = 0,
            Point position = new Point(),
            float rotation = 0f,
            float alpha = 255f,
            float scale = 1f,
            SpriteEffects orientation = SpriteEffects.None,
            int depth = 0)
        {
            builder.AddKeyframe(new LayerData(0, sheetNumber, sheetFrame, position, rotation, alpha, scale, orientation, depth));
            return builder;
        }

        public static LayerBuilder BuildKeyframe(this LayerBuilder builder,
            CopyKeyframe action = CopyKeyframe.None,
            int setKeyframe = 1)
        {
            builder.KeyframeEditing = builder.DuplicateKeyframe(action, setKeyframe);

            return builder;
        }

        public static LayerBuilder AtPosition(this LayerBuilder builder,
            int xCoordinate,
            int yCoordinate)
        {
            builder.KeyframeEditing.Position = new Point(xCoordinate, yCoordinate);
            return builder;
        }

        public static LayerBuilder WithDepth(this LayerBuilder builder,
            int depth)
        {
            builder.KeyframeEditing.Depth = depth;
            return builder;
        }

        public static LayerBuilder Flipped(this LayerBuilder builder,
            SpriteEffects flipDirection)
        {
            if (builder.KeyframeEditing.Orientation.HasFlag(flipDirection))
            {
                builder.KeyframeEditing.Orientation &= ~flipDirection;
                return builder;
            }

            builder.KeyframeEditing.Orientation |= flipDirection;
            return builder;
        }

        public static LayerBuilder NextSprite(this LayerBuilder builder,
            int positionNumber = -1)
        {
            if (positionNumber < 0)
            {
                builder.KeyframeEditing.SheetFrame++;
                return builder;
            }
            builder.KeyframeEditing.SheetFrame = positionNumber;
            return builder;
        }

        public static LayerBuilder FinishKeyframe(this LayerBuilder builder)
        {
            builder.AddKeyframe(builder.KeyframeEditing);
            return builder;
        }
    }

    public sealed class LayerBuilder
    {
        private LayerDataBuilder[] _layerData;
        private string _name;

        public LayerBuilder(string layerName)
        {
            _name = layerName;
            _layerData = new LayerDataBuilder[0];
        }

        public LayerBuilder(Layer layer,
            IEnumerable<LayerData> layerData)
        {
            _name = layer.Name;
            _layerData = layerData.Select(x => new LayerDataBuilder(x)).ToArray();
            KeyframeEditing = new LayerDataBuilder();
        }

        public LayerBuilder(LayerBuilder copy)
        {
            _name = copy._name;
            _layerData = copy._layerData;
            KeyframeEditing = new LayerDataBuilder();
        }

        public int Frames { get => _layerData.Length; }
        public string Name { get => _name; set => _name = value; }

        public LayerDataBuilder KeyframeEditing { get; set; }

        public LayerDataBuilder AddKeyframe(LayerData data)
        {
            var builder = new LayerDataBuilder(data);
            builder.AnimationKeyframe = _layerData.Length;
            ArrayUtils.Add(ref _layerData, builder);
            return builder;
        }

        public LayerDataBuilder AddKeyframe(LayerDataBuilder builder)
        {
            var newBuilder = new LayerDataBuilder(builder);
            newBuilder.AnimationKeyframe = _layerData.Length;
            ArrayUtils.Add(ref _layerData, newBuilder);
            return newBuilder;
        }

        public LayerDataBuilder DuplicateKeyframe(CopyKeyframe action, 
            int setKeyframe = 1)
        {
            switch (action)
            {
                case CopyKeyframe.New:
                case CopyKeyframe.None:
                    return new LayerDataBuilder();
                case CopyKeyframe.Last:
                    return new LayerDataBuilder(GetLatestLayerData());
                case CopyKeyframe.First:
                    return new LayerDataBuilder(GetLayerData(0));
                case CopyKeyframe.Set:
                    return new LayerDataBuilder(GetLayerData(setKeyframe - 1));
                default:
                    throw new Exception("No valid frame action selected.");
            }
        }

        public Layer Build()
        {
            Validate();
            var layerData = _layerData.Select(x => x.Build());
            return new Layer(_name, layerData);
        }

        public LayerDataBuilder GetLayerData(int index)
        {
            if (index > _layerData.Length || index < 0)
                throw new ArgumentOutOfRangeException($"Index {index} is out of the range of the current layer data array {_layerData.Length}.");
            return _layerData[index];
        }

        public bool HasLayerData(FrameIndex keyFrame)
            => _layerData.Any(x => x.AnimationKeyframe.Equals(keyFrame.Value));

        public LayerDataBuilder GetLatestLayerData()
        {
            if (!_layerData.Any())
                throw new ArgumentOutOfRangeException($"There are no latest layer data. ");
            return _layerData.Last();
        }

        public void RemoveKeyframe(int index)
        {
            if (index > _layerData.Length || index < 0)
                throw new ArgumentOutOfRangeException($"Index {index} is out of the range of the current layer data array {_layerData.Length}.");
            ArrayUtils.Remove(ref _layerData, index);
        }

        public int SetLayerData(LayerData data)
        {
            return SetLayerData(new LayerDataBuilder(data));
        }

        public int SetLayerData(LayerDataBuilder dataBuilder)
        {
            var predicate = new Func<LayerDataBuilder, bool>(x => x.AnimationKeyframe.Equals(dataBuilder.AnimationKeyframe));
            if (!_layerData.Any(predicate))
            {
                ArrayUtils.Add(ref _layerData, dataBuilder);
                return _layerData.Length - 1;
            }
            var index = Array.FindIndex(_layerData, new Predicate<LayerDataBuilder>(predicate));

            _layerData[index] = dataBuilder;
            return index;
        }

        public void Validate()
        {
            if (_layerData.GroupBy(x => x.AnimationKeyframe).Any(x => x.Count() > 1))
                throw new Exception("Cannot have multiple instaces of layer data with the same animation frame!");
        }
    }
}