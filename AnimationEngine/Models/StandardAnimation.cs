using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Services;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class StandardAnimation
        : IAnimation
    {
        public List<KeyFrame> KeyFrames { get; set; }

        public int TotalFrames { get; set; }

        public int CurrentFrame { get; set; }

        public bool IsPlaying { get; set; }

        public bool IsInitialized { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public StandardAnimation()
        {
            IsInitialized = false;
            KeyFrames = new List<KeyFrame>();
            TotalFrames = 0;
            CurrentFrame = 0;
            IsPlaying = false;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="animation">The previous animation</param>
        /// <param name="startPlaying">Whether or not to start the animation upon creation</param>
        public StandardAnimation(IAnimation animation, 
            bool startPlaying = true)
        {
            IsInitialized = false;
            CurrentFrame = animation.CurrentFrame;
            IsPlaying = startPlaying;
            TotalFrames = animation.TotalFrames;
            KeyFrames = animation.KeyFrames;
            KeyFrames.Sort();
        }

        /// <inheritdoc/>
        public int GetTotalKeyFrames()
            => KeyFrames.Count;

        /// <inheritdoc/>
        public void AdvanceFrame(int framesAdvancing = 1, bool bypassPause = false)
        {
            if (IsPlaying || bypassPause)
            {
                if (CurrentFrame + framesAdvancing < TotalFrames)
                    CurrentFrame += framesAdvancing;
                else
                    CurrentFrame = (TotalFrames - 1);
            }
        }

        /// <inheritdoc/>
        public KeyFrame GetCurrentKeyFrame()
        {
            foreach (var keyframe in KeyFrames)
            {
                if (keyframe.IsKeyFrameActive(CurrentFrame))
                    return keyframe;
            }

            throw new Exception("Error, no keyframes reflect current frame index!");
        }

        /// <inheritdoc/>
        public void ReverseFrame(int framesReversing = 1, bool bypassPause = false)
        {
            if (IsPlaying || bypassPause)
            {
                if (CurrentFrame - framesReversing >= 0)
                    CurrentFrame -= framesReversing;
                else
                    CurrentFrame = 0;
            }
        }

        /// <inheritdoc/>
        public void AddFrames(int frameAmount)
        {
            TotalFrames += frameAmount;
        }

        /// <inheritdoc/>
        public void SetCurrentFrame(int newFrame)
        {
            CurrentFrame = newFrame;
        }

        /// <inheritdoc/>
        public void StartAnimation()
        {
            IsPlaying = true;
        }

        /// <inheritdoc/>
        public void StopAnimation()
        {
            IsPlaying = false;
        }

        /// <inheritdoc/>
        public void ResetKeyFrames()
        {
            KeyFrames = new List<KeyFrame>();
        }

        /// <inheritdoc/>
        public bool CheckCurrentKeyFrame(int keyFrameIndex)
            => KeyFrames[keyFrameIndex].StartingFrameIndex.Equals(GetCurrentKeyFrame().StartingFrameIndex);

        /// <inheritdoc/>
        public int GetCurrentKeyFrameProgress()
            => CurrentFrame - GetCurrentKeyFrame().StartingFrameIndex;

        /// <inheritdoc/>
        public bool CheckCurrentKeyFrameProgress(int relativeFrame)
            => GetCurrentKeyFrameProgress().Equals(relativeFrame);

        /// <inheritdoc/>
        public void ResetAnimation(bool startPlaying = true)
        {
            StopAnimation();
            SetCurrentFrame(0);

            if (startPlaying)
                StartAnimation();
        }

        /// <inheritdoc/>
        public int GetFinalFrame()
            => TotalFrames - 1;

        public void Initialize()
        {
            if (KeyFrames.Count > 0)
                IsInitialized = true;
            else
                throw new Exception($"No Keyframes to initialize in animation!");
        }
    }
}