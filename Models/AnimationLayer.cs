using HunterCombatMR.Comparers;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace HunterCombatMR.Models
{
    public class AnimationLayer
        : IEquatable<AnimationLayer>,
        INamed
    {
        [JsonConstructor]
        public AnimationLayer(string name,
            Rectangle spriteframeRectangle,
            string texturePath,
            byte defaultDepth = 1)
        {
            Name = name;
            KeyFrames = new Dictionary<int, LayerFrameInfo>();
            SpriteFrameRectangle = spriteframeRectangle;
            DefaultDepth = defaultDepth;
            TexturePath = texturePath;
            Initialize();
        }

        public AnimationLayer(AnimationLayer copy)
        {
            Name = copy.Name;
            KeyFrames = new Dictionary<int, LayerFrameInfo>();
            SpriteFrameRectangle = copy.SpriteFrameRectangle;
            DefaultDepth = copy.DefaultDepth;
            Texture = copy.Texture;
            TexturePath = copy.TexturePath;

            foreach (var frame in copy.KeyFrames)
            {
                KeyFrames.Add(frame.Key, new LayerFrameInfo(frame.Value, frame.Value.LayerDepth));
            }

            Initialize();
        }

        /// <summary>
        /// The layer depth that should be set by default
        /// </summary>
        public byte DefaultDepth { get; set; }

        public Dictionary<int, LayerFrameInfo> KeyFrames { get; set; }

        /// <summary>
        /// Name of the layer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size and offset of the first frame the spritesheet being used. Leave x and y at 0, 0 if the sprite starts at the top-left.
        /// Saves the data of the size of each frame, but is transferred to the Texture tuple on load.
        /// </summary>
        /// <remarks>
        /// @@warn Transfer this to a texture object instead so that it can be judge by texture rather than by animation.
        /// </remarks>
        public Rectangle SpriteFrameRectangle { get; set; }

        [JsonIgnore]
        /// <summary>
        /// The texture this layer uses.
        /// </summary>
        public Texture2D Texture { get; protected set; }

        /// <summary>
        /// The path that will be used to load the texture.
        /// </summary>
        public string TexturePath { get; protected set; }

        public bool Equals(AnimationLayer other)
        {
            FrameEqualityComparer comparer = new FrameEqualityComparer();
            bool framesEqual = comparer.Equals(KeyFrames, other.KeyFrames);

            return framesEqual
                && Name.Equals(other.Name)
                && SpriteFrameRectangle.Equals(other.SpriteFrameRectangle)
                && DefaultDepth.Equals(other.DefaultDepth)
                && TexturePath.Equals(other.TexturePath);
        }

        /// <summary>
        /// This applies the layer depth to all of the frames and loads the texture, must be called before adding to an animation.
        /// </summary>
        public void Initialize()
        {
            var initializedKeyFrames = new Dictionary<int, LayerFrameInfo>();

            foreach (var keyFrame in KeyFrames)
            {
                LayerFrameInfo initializedKeyFrame = new LayerFrameInfo(keyFrame.Value, keyFrame.Value.LayerDepthOverride.HasValue ? keyFrame.Value.LayerDepthOverride.Value : DefaultDepth);
                initializedKeyFrames.Add(keyFrame.Key, initializedKeyFrame);
            }

            KeyFrames = initializedKeyFrames;

            Texture = ModContent.GetTexture(TexturePath);
        }

        public bool IsActive(int keyFrame)
                            => KeyFrames.ContainsKey(keyFrame) && KeyFrames[keyFrame].IsEnabled;

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;
            TexturePath = texture.Name;
        }

        public void UpdateLayerDepth(int amount,
                    int currentKeyFrame,
            IEnumerable<AnimationLayer> layers)
        {
            if (amount == 0)
                return;

            int newDepthInt = this.GetDepth(currentKeyFrame) + amount;
            byte newDepthByte = newDepthInt > byte.MaxValue ? byte.MaxValue : newDepthInt < byte.MinValue ? byte.MinValue : (byte)newDepthInt;
            var layerInNewPlace = layers.FirstOrDefault(x => x.KeyFrames[currentKeyFrame].LayerDepth.Equals(newDepthByte));
            if (layerInNewPlace != null)
            {
                layerInNewPlace.SetDepth(currentKeyFrame, this.GetDepth(currentKeyFrame));
            }
            this.SetDepth(currentKeyFrame, newDepthByte);
        }

        internal void AddKeyFrame(int keyFrameIndex,
            LayerFrameInfo frameInfo)
        {
            KeyFrames.Add(keyFrameIndex, frameInfo);
            Initialize();
        }

        internal void MoveKeyFrame(int keyFrameIndex,
            int newFrameIndex)
        {
            LayerFrameInfo temp = new LayerFrameInfo(KeyFrames[keyFrameIndex], KeyFrames[keyFrameIndex].LayerDepth);
            KeyFrames[keyFrameIndex] = KeyFrames[newFrameIndex];
            KeyFrames[newFrameIndex] = temp;
            Initialize();
        }

        internal void RemoveKeyFrame(int keyFrameIndex)
        {
            KeyFrames.Remove(keyFrameIndex);
            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        private void InheritPreviousKeyFrameProperties(int keyFrameIndex)
        {
            int nextFrameIndex = keyFrameIndex + 1;
            if (KeyFrames.ContainsKey(nextFrameIndex))
            {
                KeyFrames.Add(keyFrameIndex, KeyFrames[nextFrameIndex]);
                KeyFrames.Remove(nextFrameIndex);
                InheritPreviousKeyFrameProperties(nextFrameIndex);
            }
        }
    }
}