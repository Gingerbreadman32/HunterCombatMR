using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models
{
    /// <summary>
    /// Main Animator project, used to animate through a <see cref="CustomAnimation"/>
    /// </summary>
    public class Animator
    {
        private AnimatorFlags _flags;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Animator()
        {
            Initialized = false;
            KeyFrames = new SortedList<int, Keyframe>();
            CurrentFrame = FrameIndex.Zero;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">The previous animation</param>
        public Animator(Animator copy)
        {
            Flags = copy.Flags;
            CurrentFrame = copy.CurrentFrame;
        }

        /// <summary>
        /// The current frame marker
        /// </summary>
        public FrameIndex CurrentFrame { get; set; }

        /// <summary>
        /// The current keyframe
        /// </summary>
        public Keyframe CurrentKeyFrame
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

        public LoopStyle CurrentLoopStyle { get; set; }

        /// <summary>
        /// The last frame of the animation loaded
        /// </summary>
        public FrameIndex FinalFrame { get => TotalFrames - 1; }

        public AnimatorFlags Flags
        {
            get => _flags;
            set
            {
                if (!Initialized)
                {
                    HunterCombatMR.Instance?.Logger.Warn($"Animator must be initialized to set flags.");
                    _flags = AnimatorFlags.Locked;
                    return;
                }
                _flags = value;
            }
        }

        /// <summary>
        /// The amount of frames to skip per update
        /// </summary>
        public int FrameSkip { get; set; } = 0;

        public bool Initialized { get; private set; }

        /// <summary>
        /// The keyframes made from the animation's profile, in order.
        /// </summary>
        public SortedList<int, Keyframe> KeyFrames { get; }

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
            var newKeyframe = new Keyframe(KeyFrames[keyFrameIndex]);
            newKeyframe.FrameLength = newFrameLength;

            KeyFrames.Remove(keyFrameIndex);
            KeyFrames.Add(keyFrameIndex, newKeyframe);

            Initialize(new KeyFrameProfile(KeyFrames, defaultLength));
        }

        public void Initialize(KeyFrameProfile keyFrameProfile)
        {
            if (!(keyFrameProfile.KeyFrameAmount > 0))
                throw new Exception($"No Keyframes to initialize in animation!");

            if (Initialized)
                Uninitialize();

            CreateKeyFrames(keyFrameProfile);
            Initialized = true;
            Play();
        }

        public void Initialize(IEnumerable<IKeyframeData> frameData)
        {
            if (!frameData.Any())
                throw new Exception($"No Keyframes to initialize in animation!");

            if (Initialized)
                Uninitialize();

            CreateKeyFrames(frameData);
            Initialized = true;
            Play();
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
            if (!Flags.HasFlag(AnimatorFlags.Started))
                Flags |= AnimatorFlags.Started;

            if (Flags.HasFlag(AnimatorFlags.Locked))
            {
                Flags &= ~AnimatorFlags.Locked;
                return true;
            }

            return false;
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
            if (!Initialized || (Flags.HasFlag(AnimatorFlags.Locked) && !bypassLock))
                return;

            MoveFrame(FrameSkip + 1);
        }

        private void AppendKeyFrame(FrameLength keyFrameLength)
        {
            FrameIndex lastFrameIndex = TotalFrames;
            Keyframe newKeyFrame = new Keyframe(lastFrameIndex, keyFrameLength);

            newKeyFrame.StartingFrameIndex = lastFrameIndex;
            KeyFrames.Add(KeyFrames.Count, newKeyFrame);
        }

        private void CreateKeyFrames(KeyFrameProfile keyFrameProfile)
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
        }

        private void CreateKeyFrames(IEnumerable<IKeyframeData> frameData)
        {
            KeyFrames.Clear();

            foreach (var keyframe in frameData)
            {
                AppendKeyFrame(keyframe.Frames);
            }
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

        /// <inheritdoc/>
        private void MoveFrame(int amount)
        {
            if (!Initialized)
            {
                HunterCombatMR.Instance.Logger.Warn($"Animation Not Initialized");
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
    }
}