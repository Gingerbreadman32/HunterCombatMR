using HunterCombatMR.AnimationEngine.Enumerations;
using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    /// <summary>
    /// Main Animator project, used to animate through a <see cref="Animation"/>
    /// </summary>
    /// <seealso cref="Animator.Frames"/>
    public partial class Animator
    {
        private AnimatorFlags _flags;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Animator()
        {
            Initialized = false;
            KeyFrames = new SortedList<int, KeyFrame>();
            CurrentFrame = FrameIndex.Zero;
            Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">The previous animation</param>
        public Animator(Animator copy)
        {
            Flags = copy.Flags;
            CurrentFrame = copy.CurrentFrame;
            Parameters = copy.Parameters;
        }

        public LoopStyle CurrentLoopStyle { get; set; }

        public AnimatorFlags Flags
        {
            get => _flags;
            set
            {
                if (!Initialized)
                {
                    HunterCombatMR.Instance?.StaticLogger.Warn($"Animator must be initialized to set flags.");
                    _flags = AnimatorFlags.Locked;
                    return;
                }
                _flags = value;
            }
        }

        public bool Initialized { get; set; }
        public IDictionary<string, string> Parameters { get; set; }

        public void Initialize(KeyFrameProfile keyFrameProfile,
            LoopStyle loopStyle = LoopStyle.Once)
        {
            Stop(false);
            if (keyFrameProfile.KeyFrameAmount > 0)
            {
                CreateKeyFrames(keyFrameProfile, loopStyle);
                Initialized = true;
                return;
            }

            throw new Exception($"No Keyframes to initialize in animation!");
        }

        /// <summary>
        /// Pauses playback. Returns true if state was changed.
        /// </summary>
        public bool Pause()
        {
            if (!Flags.HasFlag(AnimatorFlags.Locked))
            {
                Flags |= AnimatorFlags.Locked;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts or resumes playback. Returns true if the state was changed.
        /// </summary>
        public bool Play()
        {
            if (Flags.HasFlag(AnimatorFlags.Locked))
            {
                Flags &= ~AnimatorFlags.Locked;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resumes playback if paused, pauses playback if playing
        /// </summary>
        public void PlayPause()
        {
            if (!Play())
                Pause();
        }

        public void Reinitialize(FrameLength defaultLength)
        {
            Uninitialize();

            Initialize(new KeyFrameProfile(KeyFrames, defaultLength), CurrentLoopStyle);
        }

        /// <inheritdoc/>
        public void Stop(bool replay)
        {
            Flags = AnimatorFlags.Locked;
            CurrentFrame = FrameIndex.Zero;

            if (replay)
                Play();
        }

        /// <inheritdoc/>
        public void Uninitialize()
        {
            Stop(false);
            Initialized = false;
        }

        public void Update(bool bypassLock = false)
        {
            if (!Initialized)
                return;

            if (!Flags.HasFlag(AnimatorFlags.Locked) || bypassLock)
            {
                MoveFrame(FrameSkip + 1);
            }

            if (!Flags.HasFlag(AnimatorFlags.Started) && CurrentFrame > 0)
                Flags |= AnimatorFlags.Started;
        }
    }
}