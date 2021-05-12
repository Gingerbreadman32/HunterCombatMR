using HunterCombatMR.AnimationEngine.Enumerations;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    /// <summary>
    /// Frame related methods for the <see cref="Animator"/>
    /// </summary>
    public partial class Animator
    {
        /// <summary>
        /// The current frame marker
        /// </summary>
        public FrameIndex CurrentFrame { get; set; }

        /// <summary>
        /// The current keyframe
        /// </summary>
        public KeyFrame CurrentKeyFrame
        {
            get
            {
                try
                {
                    return KeyFrames.First(x => x.Value.IsKeyFrameActive(CurrentFrame)).Value;
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException($"Error, no keyframes reflect the current frame index {CurrentFrame}!", ex);
                }
            }
        }

        /// <summary>
        /// The index of the current keyframe
        /// </summary>
        public int CurrentKeyFrameIndex
        {
            get => KeyFrames.IndexOfValue(CurrentKeyFrame);
        }

        /// <summary>
        /// The last frame of the animation loaded
        /// </summary>
        public FrameIndex FinalFrame { get => TotalFrames - 1; }

        /// <summary>
        /// The amount of frames to skip per update
        /// </summary>
        public int FrameSkip { get; set; } = 0;

        /// <summary>
        /// The keyframes made from the animation's profile, in order.
        /// </summary>
        public SortedList<int, KeyFrame> KeyFrames { get; }

        /// <summary>
        /// The total amount of frames
        /// </summary>
        public int TotalFrames
        {
            get => KeyFrames.Sum(x => x.Value.FrameLength);
        }

        public void AdjustKeyFrameLength(FrameIndex keyFrameIndex,
                    FrameLength newFrameLength,
                    FrameLength defaultLength)
        {
            var newKeyframe = new KeyFrame(KeyFrames[keyFrameIndex]);
            newKeyframe.FrameLength = newFrameLength;

            KeyFrames.Remove(keyFrameIndex);
            KeyFrames.Add(keyFrameIndex, newKeyframe);

            Reinitialize(defaultLength);
        }

        public void CreateKeyFrames(KeyFrameProfile keyFrameProfile,
                    LoopStyle loopStyle = LoopStyle.Once)
        {
            KeyFrames.Clear();

            for (var k = 0; k < keyFrameProfile.KeyFrameAmount; k++)
            {
                FrameLength frameSpeed;
                if (keyFrameProfile.KeyFrameLengths != null && keyFrameProfile.KeyFrameLengths.ContainsKey(k))
                    frameSpeed = keyFrameProfile.KeyFrameLengths[k];
                else
                    frameSpeed = keyFrameProfile.DefaultKeyFrameLength;

                AppendKeyFrame(frameSpeed);
            }

            CurrentLoopStyle = loopStyle;
        }

        /// <inheritdoc/>
        public void MoveFrame(int amount)
        {
            if (!Initialized)
            {
                HunterCombatMR.Instance.StaticLogger.Warn($"Animation Not Initialized");
                return;
            }

            FrameIndex newValue = (Flags.HasFlag(AnimatorFlags.Reversed)) ? CurrentFrame - amount : CurrentFrame + amount;

            if (newValue >= TotalFrames || newValue < 0)
            {
                Loop();
                return;
            }

            CurrentFrame = newValue;
        }

        /// <summary>
        /// Adjust the current keyframe
        /// </summary>
        /// <param name="amount">The amount of keyframes to adjust, reverse using negative integers.</param>
        public void MoveKeyFrame(int amount)
        {
            if (Initialized)
            {
                var nextKey = (CurrentKeyFrameIndex < KeyFrames.Count - 1 || CurrentKeyFrameIndex > 0)
                    ? CurrentKeyFrameIndex + amount
                    : CurrentKeyFrameIndex;

                CurrentFrame = KeyFrames[nextKey].StartingFrameIndex;
            }
        }

        private void AppendKeyFrame(FrameLength keyFrameLength)
        {
            FrameIndex lastFrameIndex = TotalFrames;
            KeyFrame newKeyFrame = new KeyFrame(lastFrameIndex, keyFrameLength);

            newKeyFrame.StartingFrameIndex = lastFrameIndex;
            KeyFrames.Add(KeyFrames.Count, newKeyFrame);
        }

        private void Loop()
        {
            switch (CurrentLoopStyle)
            {
                case LoopStyle.PlayPause:
                    CurrentFrame = (Flags.HasFlag(AnimatorFlags.Reversed)) ? FrameIndex.Zero : FinalFrame;
                    Pause();
                    break;

                case LoopStyle.Once:
                    Stop(false);
                    break;

                case LoopStyle.Loop:
                    Stop(true);
                    break;

                case LoopStyle.PingPong:
                    CurrentFrame = (Flags.HasFlag(AnimatorFlags.Reversed)) ? FrameIndex.Zero : FinalFrame;
                    Flags ^= AnimatorFlags.Reversed;
                    break;

                default:
                    throw new Exception("Loop mode not set!");
            }
        }
    }
}