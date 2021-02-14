using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class Animator
    {
        #region Public Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Animator()
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
        public Animator(Animator animation)
        {
            IsInitialized = false;
            CurrentFrame = animation.CurrentFrame;
            IsPlaying = animation.IsPlaying;
            InProgress = animation.InProgress;
            TotalFrames = animation.TotalFrames;
            KeyFrames = animation.KeyFrames;
            KeyFrames.Sort();
        }

        #endregion Public Constructors

        #region Public Properties

        public int CurrentFrame { get; set; }
        public bool InProgress { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsPlaying { get; set; }
        public List<KeyFrame> KeyFrames { get; set; }

        public LoopStyle LoopMode { get; set; }
        public int TotalFrames { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <inheritdoc/>
        public void AddFrames(int frameAmount)
        {
            TotalFrames += frameAmount;
        }

        /// <inheritdoc/>
        public void AdvanceFrame(int framesAdvancing = 1, bool bypassPause = false)
        {
            if ((IsPlaying || bypassPause) && IsInitialized)
            {
                if (CurrentFrame + framesAdvancing < TotalFrames)
                {
                    CurrentFrame += framesAdvancing;
                }
                else
                {
                    switch (LoopMode)
                    {
                        case LoopStyle.PlayPause:
                            CurrentFrame = (TotalFrames - 1);
                            PauseAnimation();
                            break;

                        case LoopStyle.Once:
                            StopAnimation();
                            break;

                        case LoopStyle.Loop:
                            ResetAnimation(true);
                            break;

                        default:
                            throw new Exception("Loop mode not set!");
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void AdvanceToNextKeyFrame()
        {
            if (IsInitialized)
            {
                var nextKey = (GetCurrentKeyFrameIndex() < GetTotalKeyFrames() - 1) ? GetCurrentKeyFrameIndex() + 1 : GetCurrentKeyFrameIndex();

                CurrentFrame = KeyFrames[nextKey].StartingFrameIndex;
            }
        }

        public void SetKeyFrame(int keyFrameIndex)
        {
            CurrentFrame = KeyFrames[keyFrameIndex].StartingFrameIndex;
        }

        /// <inheritdoc/>
        public bool CheckCurrentKeyFrame(int keyFrameIndex)
            => KeyFrames[keyFrameIndex].StartingFrameIndex.Equals(GetCurrentKeyFrame().StartingFrameIndex);

        /// <inheritdoc/>
        public bool CheckCurrentKeyFrameProgress(int relativeFrame)
            => GetCurrentKeyFrameProgress().Equals(relativeFrame);

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

        public int GetCurrentKeyFrameIndex()
                    => KeyFrames.IndexOf(GetCurrentKeyFrame());

        /// <inheritdoc/>
        public int GetCurrentKeyFrameProgress()
            => CurrentFrame - GetCurrentKeyFrame().StartingFrameIndex;

        /// <inheritdoc/>
        public int GetFinalFrame()
            => TotalFrames - 1;

        public int GetKeyFrameIndexOfFrame(int frame)
                    => KeyFrames.FindIndex(x => x.IsKeyFrameActive(frame));

        public KeyFrame GetKeyFrameOfFrame(int frame)
            => KeyFrames.First(x => x.IsKeyFrameActive(frame));

        /// <inheritdoc/>
        public int GetTotalKeyFrames()
            => KeyFrames.Count;

        public void Initialize()
        {
            if (KeyFrames.Count > 0)
                IsInitialized = true;
            else
                throw new Exception($"No Keyframes to initialize in animation!");
        }

        public void PauseAnimation()
        {
            IsPlaying = false;
            if (GetCurrentKeyFrameIndex() > 0)
                InProgress = true;
        }

        /// <inheritdoc/>
        public void ResetAnimation(bool startPlaying = true)
        {
            SetCurrentFrame(0);
            InProgress = false;

            if (startPlaying)
                StartAnimation();
        }

        /// <inheritdoc/>
        public void ResetKeyFrames()
        {
            KeyFrames = new List<KeyFrame>();
            TotalFrames = 0;
        }

        /// <inheritdoc/>
        public void ReverseFrame(int framesReversing = 1)
        {
            if (IsInitialized)
            {
                if (CurrentFrame - framesReversing >= 0)
                    CurrentFrame -= framesReversing;
                else
                    CurrentFrame = 0;
            }
        }

        /// <inheritdoc/>
        public void ReverseToPreviousKeyFrame()
        {
            if (IsInitialized)
            {
                var lastKey = (GetCurrentKeyFrameIndex() > 0) ? GetCurrentKeyFrameIndex() - 1 : GetCurrentKeyFrameIndex();

                CurrentFrame = KeyFrames[lastKey].StartingFrameIndex;
            }
        }

        /// <inheritdoc/>
        public void SetCurrentFrame(int newFrame)
        {
            CurrentFrame = newFrame;
        }

        /// <inheritdoc/>
        public void SetLoopMode(LoopStyle loopMode)
        {
            LoopMode = loopMode;
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

        /// <inheritdoc/>
        public void Uninitialize()
        {
            StopAnimation();
            IsInitialized = false;
        }

        #endregion Public Methods
    }
}