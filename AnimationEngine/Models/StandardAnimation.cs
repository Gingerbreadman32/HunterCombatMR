using HunterCombatMR.AnimationEngine.Interfaces;
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

        public bool InProgress { get; set; }

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
            InProgress = false;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="animation">The previous animation</param>
        public StandardAnimation(IAnimation animation)
        {
            IsInitialized = false;
            CurrentFrame = animation.CurrentFrame;
            IsPlaying = animation.IsPlaying;
            InProgress = animation.InProgress;
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
        public void ReverseFrame(int framesReversing = 1)
        {
            if (CurrentFrame - framesReversing >= 0)
                CurrentFrame -= framesReversing;
            else
                CurrentFrame = 0;
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
            InProgress = true;
        }

        /// <inheritdoc/>
        public void StopAnimation()
        {
            IsPlaying = false;
            InProgress = false;
            ResetAnimation(false);
        }

        public void PauseAnimation()
        {
            IsPlaying = false;
            InProgress = true;
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