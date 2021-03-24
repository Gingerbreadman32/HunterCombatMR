using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class Animation<TEntity, TAnimationType>
        : HunterCombatContentInstance,
        IAnimated,
        IModifiable, 
        IAnimation 
        where TEntity : IAnimatedEntity<TAnimationType>
        where TAnimationType : IAnimated
    {
        #region Private Fields

        private bool _modified;

        #endregion Private Fields

        #region Public Constructors

        public Animation(string name)
            : base(name)
        {
            Name = name;
        }

        #endregion Public Constructors

        #region Public Properties

        [JsonIgnore]
        public IAnimator AnimationData { get; protected set; }

        public virtual AnimationType AnimationType { get; }

        public bool IsInternal { get; internal set; }

        [JsonIgnore]
        public KeyFrameProfile KeyFrameProfile { get => LayerData.KeyFrameProfile; }

        [JsonIgnore]
        public bool IsModified
        {
            get
            {
                return _modified;
            }

            protected set
            {
                _modified = value;
            }
        }

        public LayerData LayerData { get; protected set; }
        public string Name { get; protected set; }

        public virtual IDictionary<string, string> DefaultParameters { get; } = new Dictionary<string, string>();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Duplicate an existing keyframe.
        /// </summary>
        /// <param name="duplicate">Keyframe duplicating</param>
        public void AddKeyFrame(KeyFrame duplicate)
        {
            AddKeyFrame(duplicate.FrameLength, LayerData.GetFrameInfoForLayers(AnimationData.KeyFrames.IndexOfValue(duplicate)));
        }

        /// <summary>
        /// Add a new keyframe.
        /// </summary>
        /// <param name="frameLength">Amount of frames this keyframe is active, set to -1 for default.</param>
        /// <param name="layerInfo">Layer and keyframe information if being duplicated from another keyframe.</param>
        public void AddKeyFrame(FrameLength frameLength,
            IDictionary<AnimationLayer, LayerFrameInfo> layerInfo = null)
        {
            _modified = true;
            Uninitialize();
            AnimationData.AppendKeyFrame(frameLength);

            int newIndex = AnimationData.KeyFrames.Last().Key;

            if (!frameLength.Equals(KeyFrameProfile.DefaultKeyFrameSpeed))
                KeyFrameProfile.SpecificKeyFrameSpeeds.Add(newIndex, frameLength);

            KeyFrameProfile.KeyFrameAmount++;

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

        public void AddNewLayer(AnimationLayer layerInfo)
        {
            _modified = true;
            LayerData.Layers.Add(layerInfo);
        }

        public virtual void Draw()
        {
        }

        public virtual void DrawEffects()
        {
        }

        public AnimationLayer GetLayer(string layerName)
                    => LayerData.Layers.FirstOrDefault(x => x.Name.Equals(layerName));

        /// <summary>
        /// Run this to allow this animation to run. Is run when first established and after any changes.
        /// </summary>
        public virtual void Initialize()
        {
            AnimationData.Initialize(KeyFrameProfile, LayerData.Loop);
        }

        public bool IsAnimationInitialized()
                    => AnimationData.IsInitialized;

        public void MoveKeyFrame(int keyFrameIndex,
            int newFrameIndex)
        {
            Uninitialize();
            _modified = true;

            LayerData.KeyFrameProfile.SwitchKeyFrames(keyFrameIndex, newFrameIndex);

            foreach (var layer in LayerData.Layers.Where(x => x.KeyFrames.ContainsKey(keyFrameIndex)))
            {
                layer.MoveKeyFrame(keyFrameIndex, newFrameIndex);
            }

            Initialize();
        }

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

        public void RemoveKeyFrame(int keyFrameIndex)
        {
            _modified = true;
            Uninitialize();

            AnimationData.KeyFrames.RemoveAt(keyFrameIndex);
            LayerData.KeyFrameProfile.KeyFrameAmount--;
            var speeds = LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds;

            LayerData.KeyFrameProfile.RemoveKeyFrame(keyFrameIndex);

            foreach (var layer in LayerData.Layers.Where(x => x.KeyFrames.ContainsKey(keyFrameIndex)))
            {
                layer.RemoveKeyFrame(keyFrameIndex);
            }

            Initialize();
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

        public virtual void Update(IAnimator animator)
        {
            animator.AdvanceFrame();
        }

        public void UpdateKeyFrameLength(FrameIndex keyFrameIndex, FrameLength frameAmount)
        {
            _modified = true;
            var profileModified = KeyFrameProfile.SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex);

            AnimationData.AdjustKeyFrameLength(keyFrameIndex, frameAmount, (FrameLength)KeyFrameProfile.DefaultKeyFrameSpeed);

            if (profileModified)
            {
                KeyFrameProfile.SpecificKeyFrameSpeeds.Remove(keyFrameIndex);

                if (AnimationData.KeyFrames[keyFrameIndex].FrameLength != KeyFrameProfile.DefaultKeyFrameSpeed)
                {
                    KeyFrameProfile.SpecificKeyFrameSpeeds.Add(keyFrameIndex, AnimationData.KeyFrames[keyFrameIndex].FrameLength);
                }
            }
            else if (AnimationData.KeyFrames[keyFrameIndex].FrameLength != KeyFrameProfile.DefaultKeyFrameSpeed)
            {
                KeyFrameProfile.SpecificKeyFrameSpeeds.Add(keyFrameIndex, AnimationData.KeyFrames[keyFrameIndex].FrameLength);
            }
        }

        public void UpdateLayerDepth(int amount,
            AnimationLayer layerToMove,
            IEnumerable<AnimationLayer> layers)
        {
            if (amount == 0)
                return;

            int currentKeyFrame = AnimationData.CurrentKeyFrameIndex;

            int newDepthInt = layerToMove.GetDepthAtKeyFrame(currentKeyFrame) + amount;
            byte newDepthByte = (newDepthInt > byte.MaxValue) ? byte.MaxValue : (newDepthInt < byte.MinValue) ? byte.MinValue : (byte)newDepthInt;
            var layerInNewPlace = layers.FirstOrDefault(x => x.KeyFrames[currentKeyFrame].LayerDepth.Equals(newDepthByte));
            if (layerInNewPlace != null)
            {
                layerInNewPlace.SetDepthAtKeyFrame(currentKeyFrame, layerToMove.GetDepthAtKeyFrame(currentKeyFrame));
            }
            layerToMove.SetDepthAtKeyFrame(currentKeyFrame, newDepthByte);
            _modified = true;
            // @@num:1
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        public void UpdateLayerPosition(AnimationLayer layer,
            Vector2 newPosition)
        {
            var currentKeyFrame = AnimationData.CurrentKeyFrameIndex;

            var oldPos = layer.GetPositionAtKeyFrame(currentKeyFrame);
            layer.SetPositionAtKeyFrame(currentKeyFrame, newPosition);

            if (oldPos != newPosition)
                _modified = true;
        }

        public void UpdateLayerTexture(AnimationLayer layer,
            Texture2D texture)
        {
            layer.SetTexture(texture);
            _modified = true;
        }

        public void UpdateLayerTextureFrame(AnimationLayer layer,
            int textureFrame)
        {
            layer.SetTextureFrameAtKeyFrame(AnimationData.CurrentKeyFrameIndex, textureFrame);
            _modified = true;
        }

        public void UpdateLayerVisibility(AnimationLayer layer)
        {
            _modified = true;

            layer.ToggleVisibilityAtKeyFrame(AnimationData.CurrentKeyFrameIndex);
        }

        public void UpdateLoopType(LoopStyle newLoopType)
        {
            if (IsAnimationInitialized())
            {
                _modified = true;
                AnimationData.CurrentLoopStyle = newLoopType;
                LayerData.Loop = newLoopType;
            }
        }

        #endregion Public Methods
    }
}