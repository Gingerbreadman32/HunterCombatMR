using HunterCombatMR.Comparers;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class AnimationLayer
        : IEquatable<AnimationLayer>
    {
        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Properties

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

        #endregion Public Properties

        #region Public Methods

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

        public bool GetActiveAtKeyFrame(int keyFrame)
                    => KeyFrames.ContainsKey(keyFrame) && KeyFrames[keyFrame].IsEnabled;

        public Rectangle GetCurrentFrameRectangle(int currentKeyFrame)
                    => SpriteFrameRectangle.SetSheetPositionFromFrame(KeyFrames[currentKeyFrame].SpriteFrame);

        public byte GetDepthAtKeyFrame(int keyFrame)
                    => KeyFrames[keyFrame].LayerDepth;

        public SpriteEffects GetOrientationAtKeyFrame(int keyFrame)
            => KeyFrames[keyFrame].SpriteOrientation;

        public Vector2 GetPositionAtKeyFrame(int keyFrame)
                    => KeyFrames[keyFrame].Position;

        public float GetRotationAtKeyFrame(int keyFrame)
            => KeyFrames[keyFrame].Rotation;

        public int GetSpriteTextureFrameTotal()
            => Texture.Height / SpriteFrameRectangle.Height;

        public int GetTextureFrameAtKeyFrame(int keyFrame)
                => KeyFrames[keyFrame].SpriteFrame;

        /// <summary>
        /// This applies the layer depth to all of the frames and loads the texture, must be called before adding to an animation.
        /// </summary>
        public void Initialize()
        {
            var initializedKeyFrames = new Dictionary<int, LayerFrameInfo>();

            foreach (var keyFrame in KeyFrames)
            {
                LayerFrameInfo initializedKeyFrame = new LayerFrameInfo(keyFrame.Value, (keyFrame.Value.LayerDepthOverride.HasValue) ? keyFrame.Value.LayerDepthOverride.Value : DefaultDepth);
                initializedKeyFrames.Add(keyFrame.Key, initializedKeyFrame);
            }

            KeyFrames = initializedKeyFrames;

            Texture = ModContent.GetTexture(TexturePath);
        }

        #endregion Public Methods

        #region Internal Methods

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

        internal void SetDepthAtKeyFrame(int keyFrameIndex,
            byte depth)
        {
            byte? newDepth = depth;
            KeyFrames[keyFrameIndex] = new LayerFrameInfo(KeyFrames[keyFrameIndex], newDepth.Value) { LayerDepthOverride = (newDepth == DefaultDepth) ? null : newDepth };
        }

        internal void SetPositionAtKeyFrame(int keyFrameIndex,
                            Vector2 newPosition)
        {
            KeyFrames[keyFrameIndex] = new LayerFrameInfo(KeyFrames[keyFrameIndex], KeyFrames[keyFrameIndex].LayerDepth) { Position = newPosition };
        }

        internal void SetTextureFrameAtKeyFrame(int keyFrameIndex,
            int textureFrame)
        {
            KeyFrames[keyFrameIndex] = new LayerFrameInfo(KeyFrames[keyFrameIndex], KeyFrames[keyFrameIndex].LayerDepth) { SpriteFrame = textureFrame };
        }

        internal void SetTexture(string texturePath)
        {
            TexturePath = texturePath;
            Texture = ModContent.GetTexture(texturePath);
        }

        internal void SetTexture(Texture2D texture)
        {
            Texture = texture;
            TexturePath = texture.Name;
        }

        internal void ToggleVisibilityAtKeyFrame(int keyFrameIndex)
        {
            bool visible = KeyFrames[keyFrameIndex].IsEnabled;

            visible ^= true;

            KeyFrames[keyFrameIndex] = new LayerFrameInfo(KeyFrames[keyFrameIndex], KeyFrames[keyFrameIndex].LayerDepth) { IsEnabled = visible };
        }

        #endregion Internal Methods

        #region Private Methods

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

        #endregion Private Methods
    }
}