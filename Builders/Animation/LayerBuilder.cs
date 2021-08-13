using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace HunterCombatMR.Builders.Animation
{
    public sealed class LayerBuilder
    {
        private int _depth;
        private LayerDataBuilder[] _layerData;
        private string _name;
        private string _tagName;
        private Point _tagSize;

        public LayerBuilder(string layerName,
            string textureName)
        {
            Name = layerName;
            TagName = textureName;
            _tagSize = Point.Zero;
            _layerData = new LayerDataBuilder[1] { new LayerDataBuilder() };
        }

        public int DefaultDepth { get => _depth; }
        public int Frames { get => _layerData.Length; }
        public string Name { get => _name; set => _name = value; }
        public string TagName { get => _tagName; set => _tagName = value; }
        public Point TagSize { get => _tagSize; set => _tagSize = value; }

        public Layer Build()
        {
            return new Layer(_name, _depth, new TextureTag(_tagName, _tagSize));
        }

        public void MoveKeyframe(FrameIndex keyFrameIndex,
            FrameIndex newFrameIndex)
        {
            LayerDataBuilder temp = _layerData[keyFrameIndex];
            _layerData[keyFrameIndex] = _layerData[newFrameIndex];
            _layerData[newFrameIndex] = temp;
        }

        public void RemoveKeyframe(FrameIndex keyFrameIndex)
        {
            ArrayUtils.Remove(ref _layerData, keyFrameIndex);
        }

        public void AddKeyframe(LayerDataBuilder dataBuilder)
        {
            ArrayUtils.ResizeAndFillArray(ref _layerData, _layerData.Length + 1, dataBuilder);
        }

        public void SetDepth(int depth,
            int atFrame = -1)
        {
            if (atFrame < -1 || atFrame >= Frames)
                throw new ArgumentOutOfRangeException($"{atFrame} is not a applicable frame index! Use -1 if no index is needed.");

            if (atFrame > -1)
            {
                _layerData[atFrame].DepthOverride = depth;
                return;
            }

            _depth = depth;
        }
    }
}