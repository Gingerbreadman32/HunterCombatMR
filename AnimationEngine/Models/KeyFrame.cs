using System;

namespace HunterCombatMR.AnimationEngine.Models
{
    /// <summary>
    /// Class for storing keyframe info
    /// </summary>
    public class KeyFrame
        : IComparable<KeyFrame>
    {
        /// <summary>
        /// The frame number relative to which sprite on the sprite sheet is being referenced
        /// </summary>
        /// <remarks>
        /// Starting from 0
        /// </remarks>
        public int SpriteIndex { get; }

        /// <summary>
        /// The first frame in the context of the animation that this keyframe will start at
        /// </summary>
        public int StartingFrameIndex { get; set; }

        /// <summary>
        /// The amount of frames/ticks the game will display the given keyframe
        /// </summary>
        public int FrameLength { get; set; }

        public KeyFrame (int spriteIndex, int length)
        {
            SpriteIndex = spriteIndex;
            FrameLength = length;
            StartingFrameIndex = 0;
        }

        public KeyFrame (int spriteIndex, int startingFrame, int length)
        {
            SpriteIndex = spriteIndex;
            StartingFrameIndex = startingFrame;
            FrameLength = length;
        }

        /// <summary>
        /// Gets the last frame that this keyframe is active
        /// </summary>
        /// <returns></returns>
        public int GetFinalFrameIndex()
            => StartingFrameIndex + FrameLength;

        public bool IsKeyFrameActive(int currentFrame)
            => currentFrame >= StartingFrameIndex && currentFrame < GetFinalFrameIndex();

        /// <inheritdoc/>
        public int CompareTo(KeyFrame other)
        {
            if (other != null)
                return StartingFrameIndex.CompareTo(other.StartingFrameIndex);
            else
                throw new ArgumentNullException("Compared keyframe is null!");
        }
    }
}