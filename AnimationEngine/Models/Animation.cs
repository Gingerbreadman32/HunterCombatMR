using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class Animation
        : HunterCombatContentInstance,
        INamed,
        IModifiable,
        IAnimation
    {
        public Animation(string name)
            : base(name)
        {
            Name = name;
        }

        [JsonIgnore]
        public IAnimator AnimationData { get; protected set; }

        public abstract AnimationType AnimationType { get; }

        public bool IsInitialized => AnimationData.IsInitialized;

        [JsonIgnore]
        public bool IsModified { get; set; }

        public bool IsStoredInternally { get; internal set; }

        [JsonIgnore]
        public KeyFrameProfile KeyFrameProfile { get => LayerData.KeyFrameProfile; }

        public LayerData LayerData { get; protected set; }
        public string Name { get; protected set; }

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
            IsModified = true;
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

        /// <summary>
        /// Run this to allow this animation to run. Is run when first established and after any changes.
        /// </summary>
        public virtual void Initialize()
        {
            AnimationData.Initialize(KeyFrameProfile, LayerData.Loop);
        }

        public void ModifyAnimation(Action action,
                    bool reinitialize = false)
        {
            IsModified = true;
            if (reinitialize)
                Uninitialize();

            action.Invoke();

            if (reinitialize)
                Initialize();
        }

        public void MoveKeyFrame(int keyFrameIndex,
            int newFrameIndex)
        {
            Uninitialize();
            IsModified = true;

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
            if (!AnimationData.IsPlaying && IsInitialized)
                AnimationData.StartAnimation();
        }

        public void RemoveKeyFrame(int keyFrameIndex)
        {
            IsModified = true;
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

        public void Stop()
        {
            Pause();
            AnimationData.ResetAnimation(false);
        }

        /// <summary>
        /// Run this before making changes to an animation.
        /// </summary>
        public virtual void Uninitialize()
        {
            AnimationData.Uninitialize();
        }

        public virtual void Update()
        {
            AnimationData.AdvanceFrame();
        }

        public void UpdateKeyFrameLength(FrameIndex keyFrameIndex, FrameLength frameAmount)
        {
            IsModified = true;
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

        public void UpdateLoopType(LoopStyle newLoopType)
        {
            if (IsInitialized)
            {
                IsModified = true;
                AnimationData.CurrentLoopStyle = newLoopType;
                LayerData.Loop = newLoopType;
            }
        }
    }
}