﻿using HunterCombatMR.Comparers;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class AnimationLayer
        : IEquatable<AnimationLayer>
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

        [JsonConstructor]
        public AnimationLayer(string name,
            Rectangle spriteframeRectangle,
            byte defaultDepth = 1)
        {
            Name = name;
            Frames = new Dictionary<int, LayerFrameInfo>();
            SpriteFrameRectangle = spriteframeRectangle;
            DefaultDepth = defaultDepth;
        }

        public AnimationLayer(AnimationLayer copy)
        {
            Name = copy.Name;
            Frames = new Dictionary<int, LayerFrameInfo>();
            SpriteFrameRectangle = copy.SpriteFrameRectangle;
            DefaultDepth = copy.DefaultDepth;

            foreach (var frame in copy.Frames)
            {
                Frames.Add(frame.Key, new LayerFrameInfo(frame.Value, frame.Value.LayerDepth));
            }
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

        public void SetPositionAtFrame(int frame,
            Vector2 newPosition)
        {
            Frames[frame] = new LayerFrameInfo(Frames[frame], Frames[frame].LayerDepth) { Position = newPosition };
        }

        public Rectangle GetCurrentFrameRectangle(int currentKeyFrame)
            => SpriteFrameRectangle.SetSheetPositionFromFrame(Frames[currentKeyFrame].SpriteFrame);

        public bool Equals(AnimationLayer other)
        {
            FrameEqualityComparer comparer = new FrameEqualityComparer();
            bool framesEqual = comparer.Equals(Frames, other.Frames);

            return framesEqual
                && Name.Equals(other.Name)
                && SpriteFrameRectangle.Equals(other.SpriteFrameRectangle)
                && DefaultDepth.Equals(other.DefaultDepth);
        }
    }
}