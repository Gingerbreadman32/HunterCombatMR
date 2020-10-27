using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class AnimationLayer
    {
        public Dictionary<int, LayerFrameInfo> Frames { get; set; }

        /// <summary>
        /// Name of the layer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size and offset of the first frame the spritesheet being used. Leave x and y at 0, 0 if the sprite starts at the top-left.
        /// </summary>
        public Rectangle SpriteFrameRectangle { get; set; }

        /// <summary>
        /// The layer depth that should be set by default
        /// </summary>
        public byte DefaultDepth { get; set; }

        public AnimationLayer(string name,
            Rectangle frameRect,
            byte defaultDepth = 1)
        {
            Name = name;
            Frames = new Dictionary<int, LayerFrameInfo>();
            SpriteFrameRectangle = frameRect;
            DefaultDepth = defaultDepth;
        }

        /// <summary>
        /// This applies the layer depth to all of the frames, must be called before adding to an animation.
        /// </summary>
        public void Initialize()
        {
            var initializedFrames = new Dictionary<int, LayerFrameInfo>();

            foreach (var frame in Frames)
            {
                LayerFrameInfo initializedFrame = new LayerFrameInfo(frame.Value, (frame.Value.LayerDepthOverride.HasValue) ? frame.Value.LayerDepthOverride.Value : DefaultDepth);
                initializedFrames.Add(frame.Key, initializedFrame);
            }

            Frames = initializedFrames;
        }

        public Rectangle GetCurrentFrameRectangle(int currentKeyFrame)
            => SpriteFrameRectangle.SetSheetPositionFromFrame(Frames[currentKeyFrame].SpriteFrame);
    }
}