using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class Animation
        : IAnimated
    {
        public string Name { get; protected set; }
        public LayerData LayerData { get; protected set; }
        [JsonIgnore]
        public AnimatedData AnimationData { get; protected set; }

        public virtual void Initialize()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(AnimationData, LayerData.KeyFrameProfile, false, LayerData.Loop);
        }

        public void AddNewLayer(AnimationLayer layerInfo)
        {
            LayerData.Layers.Add(layerInfo);
        }

        public virtual void Update()
        {
            AnimationData.AdvanceFrame();
        }

        public void Pause()
        {
            if (AnimationData.IsPlaying)
                AnimationData.PauseAnimation();
        }

        public void Play()
        {
            if (!AnimationData.IsPlaying)
                AnimationData.StartAnimation();
        }

        public void Stop()
        {
            Pause();
            AnimationData.ResetAnimation(false);
        }

        public void Restart()
        {
            AnimationData.ResetAnimation(true);
        }

        public void UpdateKeyFrameLength(int keyFrameIndex, int frameAmount, bool setAmount = false, bool setDefault = false)
        {
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
            AnimationData.SetLoopMode(newLoopType);
            LayerData.Loop = newLoopType;
        }
    }
}
