using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public partial class Animator<TAnimated, TObject, TObjectAnimated> 
        : IAnimator 
        where TAnimated : IAnimated
        where TObject : IAnimatedEntity<TObjectAnimated>
        where TObjectAnimated : IAnimated
    {
        private SortedList<int, KeyFrame> _keyFrames;

        #region Public Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Animator()
        {
            IsInitialized = false;
            _keyFrames = new SortedList<int, KeyFrame>();
            CurrentFrame = 0;
            IsPlaying = false;
            InProgress = false;
            Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">The previous animation</param>
        public Animator(IAnimator copy)
        {
            IsInitialized = false;
            CurrentFrame = copy.CurrentFrame;
            IsPlaying = copy.IsPlaying;
            InProgress = copy.InProgress;
            _keyFrames = copy.KeyFrames;
            Parameters = new Dictionary<string, string>();
        }

        #endregion Public Constructors

        #region Public Properties

        public int CurrentFrame { get; set; }
        public bool InProgress { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsPlaying { get; set; }
        public LoopStyle CurrentLoopStyle { get; set; }
        public SortedList<int, KeyFrame> KeyFrames { get => _keyFrames; }
        public IDictionary<string, string> Parameters { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void CreateKeyFrames(KeyFrameProfile keyFrameProfile,
            LoopStyle loopStyle = LoopStyle.Once)
        {
            CurrentFrame = 0;
            _keyFrames.Clear();

            for (var k = 0; k < keyFrameProfile.KeyFrameAmount; k++)
            {
                FrameLength frameSpeed;
                if (keyFrameProfile.SpecificKeyFrameSpeeds != null && keyFrameProfile.SpecificKeyFrameSpeeds.ContainsKey(k))
                    frameSpeed = keyFrameProfile.SpecificKeyFrameSpeeds[k];
                else
                    frameSpeed = (FrameLength)keyFrameProfile.DefaultKeyFrameSpeed;

                AppendKeyFrame(frameSpeed);
            }

            CurrentLoopStyle = loopStyle;
        }

        public void AppendKeyFrame(FrameLength keyFrameLength)
        {
            KeyFrame newKeyFrame = new KeyFrame((FrameIndex)TotalFrames, keyFrameLength);

            if (newKeyFrame.FrameLength > 0)
            {
                newKeyFrame.StartingFrameIndex = (FrameIndex)TotalFrames;
                KeyFrames.Add(KeyFrames.Count, newKeyFrame);
            }
            else
            {
                throw new Exception("Keyframe must be longer than 0 frames!");
            }
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
                    switch (CurrentLoopStyle)
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

                        case LoopStyle.PingPong:
                            HunterCombatMR.Instance.StaticLogger.Debug($"Ping Pong loop not implemented.");
                            throw new Exception("Loop mode failure!");

                        default:
                            throw new Exception("Loop mode not set!");
                    }
                }
            }
            else if (!IsInitialized)
                HunterCombatMR.Instance.StaticLogger.Warn($"Animation Not Initialized");
        }

        /// <inheritdoc/>
        public void AdvanceToNextKeyFrame()
        {
            if (IsInitialized)
            {
                var nextKey = (CurrentKeyFrameIndex < KeyFrames.Count - 1) ? CurrentKeyFrameIndex + 1 : CurrentKeyFrameIndex;

                CurrentFrame = KeyFrames[nextKey].StartingFrameIndex;
            }
        }

        /// <inheritdoc/>
        public bool CheckCurrentKeyFrame(int keyFrameIndex)
            => KeyFrames[keyFrameIndex].StartingFrameIndex.Equals(GetCurrentKeyFrame().StartingFrameIndex);

        /// <inheritdoc/>
        public KeyFrame GetCurrentKeyFrame()
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

        public void Initialize(KeyFrameProfile keyFrameProfile,
            LoopStyle loopStyle = LoopStyle.Once)
        {
            if (keyFrameProfile.KeyFrameAmount > 0)
            {
                CreateKeyFrames(keyFrameProfile, loopStyle);
                IsInitialized = true;
                return;
            }

            throw new Exception($"No Keyframes to initialize in animation!");
        }

        public void PauseAnimation()
        {
            IsPlaying = false;
            if (CurrentKeyFrameIndex > 0)
                InProgress = true;
        }

        /// <inheritdoc/>
        public void ResetAnimation(bool startPlaying = true)
        {
            CurrentFrame = 0;
            InProgress = false;

            if (startPlaying)
                StartAnimation();
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
                var lastKey = (CurrentKeyFrameIndex > 0) ? CurrentKeyFrameIndex - 1 : CurrentKeyFrameIndex;

                CurrentFrame = KeyFrames[lastKey].StartingFrameIndex;
            }
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