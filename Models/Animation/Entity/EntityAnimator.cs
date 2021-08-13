using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Exceptions;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Animation.Entity
{
    /// <summary>
    /// Stores and manipulates the current animation data for the component it is attached to's entity.
    /// </summary>
    public class EntityAnimator
    {
        private AnimationFlags _flags;
        private int _frameSkip;
        private bool _initialized;
        private Keyframe[] _keyframes;
        private FrameIndex[] _keyframeTable;
        private bool _playing;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EntityAnimator()
        {
            _keyframes = new Keyframe[0];
            Frame = FrameIndex.Zero;
            _keyframeTable = new FrameIndex[0];
        }

        public EntityAnimator(EntityAnimation animation)
        {
            Frame = FrameIndex.Zero;
            _keyframeTable = new FrameIndex[0];
            Initialize(animation.Layers.FrameData);
        }

        /// <summary>
        /// Various flags that modify the behavior of the animation.
        /// </summary>
        public AnimationFlags Flags { get => _flags; }

        /// <summary>
        /// The current frame of the animation
        /// </summary>
        public FrameIndex Frame { get; set; }

        /// <summary>
        /// The amount of frames to skip per update
        /// </summary>
        public int FrameSkip { get => _frameSkip; set => _frameSkip = value; }

        /// <summary>
        /// Whether or not an animation has been set.
        /// </summary>
        public bool Initialized { get => _initialized; }

        /// <summary>
        /// The keyframes made from the animation data, in order.
        /// </summary>
        public Keyframe[] Keyframes { get => _keyframes; }

        /// <summary>
        /// The way that the animation will or will not be looped.
        /// </summary>
        public LoopStyle LoopStyle { get; set; }

        /// <summary>
        /// The index of the current keyframe
        /// </summary>
        public FrameIndex GetCurrentKeyFrame()
        {
            if (!_initialized)
                throw new AnimatorInitializedException("Cannot get current key frame.");

            return _keyframeTable[Frame];
        }

        /// <summary>
        /// The last frame of the animation loaded
        /// </summary>
        public FrameIndex GetFinalFrame()
        {
            if (!_initialized)
                throw new AnimatorInitializedException("Cannot get final frame.");

            return GetTotalFrames() - 1;
        }

        /// <summary>
        /// The total amount of frames among all keyframes.
        /// </summary>
        public int GetTotalFrames()
            => _keyframeTable.Length;

        public void Initialize(IEnumerable<IKeyframeData> frameData)
        {
            if (!frameData.Any())
                throw new AnimatorInitializationException($"No Keyframes to initialize!");

            try
            {
                if (_initialized)
                    Uninitialize();

                CreateKeyFrames(frameData);
                _initialized = true;
                Play();
            }
            catch (Exception ex)
            {
                throw new AnimatorInitializationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause()
        {
            if (!_flags.HasFlag(AnimationFlags.Paused))
            {
                _flags |= AnimationFlags.Paused;
            }
        }

        /// <summary>
        /// Starts or resumes playback.
        /// </summary>
        public void Play()
        {
            if (_flags.HasFlag(AnimationFlags.Paused))
                _flags &= AnimationFlags.Paused;

            _playing = true;
        }

        /// <inheritdoc/>
        public void Stop(bool replay)
        {
            if (_flags.HasFlag(AnimationFlags.Paused))
                _flags &= AnimationFlags.Paused;

            Frame = FrameIndex.Zero;
            _playing = false;

            if (replay)
                Play();
        }

        /// <inheritdoc/>
        public void Uninitialize()
        {
            Stop(false);
            _initialized = false;
            _keyframes = new Keyframe[0];
            _keyframeTable = new FrameIndex[0];
        }

        public void Update()
        {
            if (!_initialized)
            {
                if (_flags != AnimationFlags.None || _playing || Frame > FrameIndex.Zero)
                    Uninitialize();
                return;
            }

            AdvanceFrame(FrameSkip + 1);
        }

        /// <inheritdoc/>
        private void AdvanceFrame(int frames = 1)
        {
            if (!_initialized)
                throw new AnimatorInitializedException("Cannot advance frame!");

            if (_flags.HasFlag(AnimationFlags.Paused) || !_playing)
                return;

            FrameIndex calculatedFrames = Flags.HasFlag(AnimationFlags.Reversed)
                ? Frame - frames
                : Frame + frames;

            if (calculatedFrames >= GetTotalFrames() || calculatedFrames < 0)
            {
                Loop();
                return;
            }

            Frame = calculatedFrames;
        }

        private void CreateKeyFrames(IEnumerable<IKeyframeData> frameData)
        {
            _keyframes = frameData.Cast<Keyframe>().ToArray();

            ArrayUtils.ResizeAndFillArray(ref _keyframeTable, _keyframes.Sum(x => x.Frames), 0);

            if (_keyframeTable.Length < 1)
                throw new ArgumentException("No keyframes have any length to attribute!");

            SetKeyframeTable(0, 0);
        }

        private void Loop()
        {
            switch (LoopStyle)
            {
                case LoopStyle.PlayPause:
                    Frame = _flags.HasFlag(AnimationFlags.Reversed) ? FrameIndex.Zero : GetFinalFrame();
                    Pause();
                    break;

                case LoopStyle.Once:
                    Stop(false);
                    break;

                case LoopStyle.Loop:
                    Stop(true);
                    break;

                case LoopStyle.PingPong:
                    Frame = _flags.HasFlag(AnimationFlags.Reversed) ? FrameIndex.Zero : GetFinalFrame();
                    _flags ^= AnimationFlags.Reversed;
                    break;

                default:
                    throw new Exception("Loop mode not set!");
            }
        }

        private void SetKeyframeTable(FrameIndex index,
            int keyframe)
        {
            for (int f = index; f < _keyframes[keyframe].Frames; f++)
            {
                _keyframeTable[f] = keyframe;
                index = f;
            }

            keyframe++;

            if (keyframe < _keyframes.Length && keyframe < AnimationConstants.MaxKeyframes)
                SetKeyframeTable(keyframe, index);
        }
    }
}