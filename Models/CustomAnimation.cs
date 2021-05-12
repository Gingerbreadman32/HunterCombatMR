using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models
{
    public abstract class CustomAnimation
        : HunterCombatContentInstance,
        INamed,
        IModifiable,
        ICustomAnimation
    {
        public CustomAnimation(string name)
            : base(name)
        {
            Name = name;
        }

        [JsonIgnore]
        public Animator AnimationData { get; protected set; }

        public abstract AnimationType AnimationType { get; }

        public bool IsInitialized { get => AnimationData.Initialized; }

        [JsonIgnore]
        public bool IsModified { get; set; }

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
        /// <param name="layerInfo">
        /// Layer and keyframe information if being duplicated from another keyframe.
        /// </param>
        public void AddKeyFrame(FrameLength frameLength,
            IDictionary<AnimationLayer, LayerFrameInfo> layerInfo = null)
        {
            IsModified = true;
            Uninitialize();

            int newIndex = AnimationData.KeyFrames.Last().Key + 1;

            if (!frameLength.Equals(KeyFrameProfile.DefaultKeyFrameLength))
                KeyFrameProfile.KeyFrameLengths.Add(newIndex, frameLength);

            KeyFrameProfile.KeyFrameAmount += 1;

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

        public void RemoveKeyFrame(int keyFrameIndex)
        {
            IsModified = true;
            Uninitialize();

            AnimationData.KeyFrames.RemoveAt(keyFrameIndex);
            LayerData.KeyFrameProfile.KeyFrameAmount -= 1;
            var speeds = LayerData.KeyFrameProfile.KeyFrameLengths;

            LayerData.KeyFrameProfile.RemoveKeyFrame(keyFrameIndex);

            foreach (var layer in LayerData.Layers.Where(x => x.KeyFrames.ContainsKey(keyFrameIndex)))
            {
                layer.RemoveKeyFrame(keyFrameIndex);
            }

            Initialize();
        }

        /// <summary>
        /// Run this before making changes to an animation.
        /// </summary>
        public virtual void Uninitialize()
        {
            AnimationData.Uninitialize();
        }

        public void Update()
        {
            AnimationData.Update();
        }

        public void UpdateKeyFrameLength(FrameIndex keyFrameIndex, FrameLength frameAmount)
        {
            IsModified = true;
            var profileModified = KeyFrameProfile.KeyFrameLengths.ContainsKey(keyFrameIndex);

            AnimationData.AdjustKeyFrameLength(keyFrameIndex, frameAmount, KeyFrameProfile.DefaultKeyFrameLength);

            if (profileModified)
            {
                KeyFrameProfile.KeyFrameLengths.Remove(keyFrameIndex);

                if (AnimationData.KeyFrames[keyFrameIndex].FrameLength != KeyFrameProfile.DefaultKeyFrameLength)
                {
                    KeyFrameProfile.KeyFrameLengths.Add(keyFrameIndex, AnimationData.KeyFrames[keyFrameIndex].FrameLength);
                }
            }
            else if (AnimationData.KeyFrames[keyFrameIndex].FrameLength != KeyFrameProfile.DefaultKeyFrameLength)
            {
                KeyFrameProfile.KeyFrameLengths.Add(keyFrameIndex, AnimationData.KeyFrames[keyFrameIndex].FrameLength);
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