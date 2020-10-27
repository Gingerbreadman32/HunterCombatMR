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
        /// The order this keyframe will be positioned
        /// </summary>
        public int KeyFrameOrder { get; set; }

        /// <summary>
        /// The first frame in the context of the animation that this keyframe will start at
        /// </summary>
        public int StartingFrameIndex { get; set; }

        /// <summary>
        /// The amount of frames/ticks the game will display the given keyframe
        /// </summary>
        public int FrameLength { get; set; }

        public KeyFrame (int length)
        {
            FrameLength = length;
            StartingFrameIndex = 0;
            KeyFrameOrder = 0;
        }

        public KeyFrame (int startingFrame, 
            int length,
            int order)
        {
            StartingFrameIndex = startingFrame;
            FrameLength = length;
            KeyFrameOrder = order;
        }

        public KeyFrame(KeyFrame copy)
        {
            StartingFrameIndex = copy.StartingFrameIndex;
            FrameLength = copy.FrameLength;
            KeyFrameOrder = copy.KeyFrameOrder;
        }

        /// <summary>
        /// Gets the last frame that this keyframe is active
        /// </summary>
        /// <returns></returns>
        public int GetFinalFrameIndex()
            => StartingFrameIndex + FrameLength;

        /// <summary>
        /// Gets whether the keyframe is currently active in the given animation
        /// </summary>
        /// <param name="currentFrame">The current frame index of the animation</param>
        /// <returns>Yes if the keyframe is active</returns>
        public bool IsKeyFrameActive(int currentFrame)
            => currentFrame >= StartingFrameIndex && currentFrame < GetFinalFrameIndex();

        /// <inheritdoc/>
        public int CompareTo(KeyFrame other)
        {
            if (other != null)
                return KeyFrameOrder.CompareTo(other.KeyFrameOrder);
            else
                throw new ArgumentNullException("Compared keyframe is null!");
        }
    }
}