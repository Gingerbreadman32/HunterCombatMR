using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class Animation
        : IAnimated,
        IModifiable
    {
        #region Protected Fields

        protected bool _modified;

        #endregion Protected Fields

        #region Public Properties

        [JsonIgnore]
        public AnimatedData AnimationData { get; protected set; }

        public abstract AnimationType AnimationType { get; }

        public bool IsInternal { get; internal set; }

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

        public AnimationLayer GetLayer(string layerName)
                    => LayerData.Layers.FirstOrDefault(x => x.Name.Equals(layerName));

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
            var profileModified = LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex);

            if (setDefault)
            {
                setAmount = true;
                frameAmount = LayerData.KeyFrameProfile.DefaultKeyFrameSpeed;
            }

            HunterCombatMR.Instance.AnimationKeyFrameManager.AdjustKeyFrameLength(AnimationData,
                        keyFrameIndex,
                        frameAmount,
                        !setAmount);

            if (profileModified)
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

        public void AddKeyFrame(KeyFrame duplicate,
            IDictionary<AnimationLayer, LayerFrameInfo> layerInfo)
        {
            AddKeyFrame(duplicate.FrameLength, layerInfo);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="frameLength">Amount of frames this keyframe is active, set to -1 for default.</param>
        /// <param name="layerInfo"></param>
        public void AddKeyFrame(int frameLength = -1,
            IDictionary<AnimationLayer, LayerFrameInfo> layerInfo = null)
        {
            _modified = true;
            bool defaultSpeed = frameLength == -1;
            Uninitialize();
            HunterCombatMR.Instance.AnimationKeyFrameManager.AppendKeyFrame(AnimationData, (defaultSpeed) ? LayerData.KeyFrameProfile.DefaultKeyFrameSpeed : frameLength);

            int newIndex = AnimationData.KeyFrames.Last().KeyFrameOrder;

            if (!defaultSpeed)
                LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.Add(newIndex, frameLength);

            LayerData.KeyFrameProfile.KeyFrameAmount++;

            if (layerInfo != null)
            {
                foreach (var layer in layerInfo)
                {
                    layer.Key.AddKeyFrame(newIndex, layer.Value);
                }
            }
            else
            {
                foreach (var layer in LayerData.Layers)
                {
                    layer.AddKeyFrame(newIndex, new LayerFrameInfo(0, Vector2.Zero));
                }
            }

            Initialize();
        }

        public void UpdateLayerDepth(int amount,
            AnimationLayer layerToMove,
            IEnumerable<AnimationLayer> layers)
        {
            if (amount == 0)
                return;

            int currentKeyFrame = AnimationData.GetCurrentKeyFrameIndex();

            int newDepthInt = layerToMove.GetDepthAtKeyFrame(currentKeyFrame) + amount;
            byte newDepthByte = (newDepthInt > byte.MaxValue) ? byte.MaxValue : (newDepthInt < byte.MinValue) ? byte.MinValue : (byte)newDepthInt;
            var layerInNewPlace = layers.FirstOrDefault(x => x.KeyFrames[currentKeyFrame].LayerDepth.Equals(newDepthByte));
            if (layerInNewPlace != null)
            {
                layerInNewPlace.SetDepthAtKeyFrame(currentKeyFrame, layerToMove.GetDepthAtKeyFrame(currentKeyFrame));
            }
            layerToMove.SetDepthAtKeyFrame(currentKeyFrame, newDepthByte);
            _modified = true;
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        public void UpdateLayerPosition(AnimationLayer layerToMove,
            Vector2 newPosition)
        {
            var currentKeyFrame = AnimationData.GetCurrentKeyFrameIndex();

            var oldPos = layerToMove.GetPositionAtKeyFrame(currentKeyFrame);
            layerToMove.SetPositionAtKeyFrame(currentKeyFrame, newPosition);

            if (oldPos != newPosition)
                _modified = true;
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

        public void UpdateLayerVisibility(AnimationLayer layer)
        {
            _modified = true;

            layer.ToggleVisibilityAtKeyFrame(AnimationData.GetCurrentKeyFrameIndex());
        }

        #endregion Public Methods
    }
}