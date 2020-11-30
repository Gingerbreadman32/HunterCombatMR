using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using Newtonsoft.Json;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class Animation
        : IAnimated
    {
        #region Private Fields

        private bool _modified;

        #endregion Private Fields

        #region Public Properties

        [JsonIgnore]
        public AnimatedData AnimationData { get; protected set; }

        public abstract AnimationType AnimationType { get; }

        [JsonIgnore]
        public bool IsModified
        {
            get
            {
                return _modified;
            }
        }

        public LayerData LayerData { get; protected set; }
        public string Name { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        public void AddNewLayer(AnimationLayer layerInfo)
        {
            LayerData.Layers.Add(layerInfo);
        }

        public virtual void Draw()
        {
        }

        public virtual void DrawEffects()
        {
        }

        public abstract Animation Duplicate(string name);

        /// <summary>
        /// Run this to allow this animation to run. Is run when first established and after any changes.
        /// </summary>
        public virtual void Initialize()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(AnimationData, LayerData.KeyFrameProfile, false, LayerData.Loop);
        }

        public bool IsAnimationInitialized()
                    => AnimationData.IsInitialized;

        public void Pause()
        {
            if (AnimationData.IsPlaying)
                AnimationData.PauseAnimation();
        }

        public void Play()
        {
            if (!AnimationData.IsPlaying && IsAnimationInitialized())
                AnimationData.StartAnimation();
        }

        public void Restart()
        {
            AnimationData.ResetAnimation(true);
        }

        public void Stop()
        {
            Pause();
            AnimationData.ResetAnimation(false);
        }

        /// <summary>
        /// Run this if you need to make changes to an animation and want to prevent anything from affecting it.
        /// </summary>
        public virtual void Uninitialize()
        {
            AnimationData.Uninitialize();
        }

        public virtual void Update()
        {
            AnimationData.AdvanceFrame();
        }

        public void UpdateKeyFrameLength(int keyFrameIndex, int frameAmount, bool setAmount = false, bool setDefault = false)
        {
            _modified = true;
            var isModified = LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex);

            if (setDefault)
            {
                setAmount = true;
                frameAmount = LayerData.KeyFrameProfile.DefaultKeyFrameSpeed;
            }

            HunterCombatMR.Instance.AnimationKeyFrameManager.AdjustKeyFrameLength(AnimationData,
                        keyFrameIndex,
                        frameAmount,
                        !setAmount);

            if (isModified)
            {
                LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.Remove(keyFrameIndex);

                if (!setDefault && AnimationData.KeyFrames[keyFrameIndex].FrameLength != LayerData.KeyFrameProfile.DefaultKeyFrameSpeed)
                {
                    LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.Add(keyFrameIndex, AnimationData.KeyFrames[keyFrameIndex].FrameLength);
                }
            }
            else if (AnimationData.KeyFrames[keyFrameIndex].FrameLength != LayerData.KeyFrameProfile.DefaultKeyFrameSpeed)
            {
                LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.Add(keyFrameIndex, AnimationData.KeyFrames[keyFrameIndex].FrameLength);
            }
        }

        public void UpdateLoopType(LoopStyle newLoopType)
        {
            if (IsAnimationInitialized())
            {
                _modified = true;
                AnimationData.SetLoopMode(newLoopType);
                LayerData.Loop = newLoopType;
            }
        }

        #endregion Public Methods
    }
}