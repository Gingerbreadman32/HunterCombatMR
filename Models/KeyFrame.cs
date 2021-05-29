using HunterCombatMR.Extensions;
using System;

namespace HunterCombatMR.Models
{
    /// <summary>
    /// Object for storing keyframe info
    /// </summary>
    public struct Keyframe
        : IComparable<Keyframe>
    {
        public Keyframe(FrameLength length)
        {
            FrameLength = length;
            StartingFrameIndex = 0;
        }

        public Keyframe(FrameIndex startingFrame,
                    FrameLength length)
        {
            StartingFrameIndex = startingFrame;
            FrameLength = length;
        }

        public Keyframe(Keyframe copy)
        {
            StartingFrameIndex = copy.StartingFrameIndex;
            FrameLength = copy.FrameLength;
        }

        /// <summary>
        /// Gets the last frame that this keyframe is active
        /// </summary>
        /// <returns>The final frame active</returns>
        public FrameIndex FinalFrameIndex
        {
            get => StartingFrameIndex + (FrameLength - 1);
        }

        /// <summary>
        /// The amount of frames/ticks the game will display the given keyframe
        /// </summary>
        public FrameLength FrameLength { get; set; }

        /// <summary>
        /// The first frame in the context of the animation that this keyframe will start at
        /// </summary>
        public FrameIndex StartingFrameIndex { get; set; }

        public static bool operator <=(Keyframe a, Keyframe b)
            => a.StartingFrameIndex <= b.StartingFrameIndex;

        public static bool operator >=(Keyframe a, Keyframe b)
            => a.StartingFrameIndex >= b.StartingFrameIndex;

        /// <inheritdoc/>
        public int CompareTo(Keyframe other)
            => other.StartingFrameIndex.CompareTo(other.StartingFrameIndex);

        public FrameIndex GetFrameProgress(FrameIndex currentFrame)
        {
            if (!IsKeyFrameActive(currentFrame))
            {
                HunterCombatMR
                    .Instance
                    .StaticLogger
                    .Error($"Current animation frame {currentFrame} does not include keyframe that starts at {StartingFrameIndex}.");
                return FrameIndex.Zero;
            }

            return currentFrame - StartingFrameIndex;
        }

        /// <summary>
        /// Gets whether the keyframe is currently active in the given animation
        /// </summary>
        /// <param name="currentFrame">The current frame index of the animation</param>
        /// <returns>Yes if the keyframe is active</returns>
        public bool IsKeyFrameActive(FrameIndex currentFrame)
            => currentFrame >= StartingFrameIndex && currentFrame <= FinalFrameIndex;
    }
}