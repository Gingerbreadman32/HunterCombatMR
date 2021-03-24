using System;

namespace HunterCombatMR.AnimationEngine.Models
{
    /// <summary>
    /// Object for storing keyframe info
    /// </summary>
    public struct KeyFrame
        : IComparable<KeyFrame>
    {
        #region Public Constructors

        public KeyFrame(FrameLength length)
        {
            FrameLength = length;
            StartingFrameIndex = (FrameIndex)0;
        }

        public KeyFrame(FrameIndex startingFrame,
                    FrameLength length)
        {
            StartingFrameIndex = startingFrame;
            FrameLength = length;
        }

        public KeyFrame(KeyFrame copy)
        {
            StartingFrameIndex = copy.StartingFrameIndex;
            FrameLength = copy.FrameLength;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the last frame that this keyframe is active
        /// </summary>
        /// <returns>The final frame active</returns>
        public FrameIndex FinalFrameIndex
        {
            get => (FrameIndex)(StartingFrameIndex + (FrameLength - 1));
        }

        /// <summary>
        /// The amount of frames/ticks the game will display the given keyframe
        /// </summary>
        public FrameLength FrameLength { get; set; }

        /// <summary>
        /// The first frame in the context of the animation that this keyframe will start at
        /// </summary>
        public FrameIndex StartingFrameIndex { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static bool operator <=(KeyFrame a, KeyFrame b)
            => a.StartingFrameIndex <= b.StartingFrameIndex;

        public static bool operator >=(KeyFrame a, KeyFrame b)
            => a.StartingFrameIndex >= b.StartingFrameIndex;

        /// <inheritdoc/>
        public int CompareTo(KeyFrame other)
            => other.StartingFrameIndex.CompareTo(other.StartingFrameIndex);

        /// <summary>
        /// Gets whether the keyframe is currently active in the given animation
        /// </summary>
        /// <param name="currentFrame">The current frame index of the animation</param>
        /// <returns>Yes if the keyframe is active</returns>
        public bool IsKeyFrameActive(int currentFrame)
            => currentFrame >= StartingFrameIndex && currentFrame <= FinalFrameIndex;

        #endregion Public Methods
    }
}