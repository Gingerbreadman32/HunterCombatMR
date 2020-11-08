using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using Newtonsoft.Json;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class LayeredAnimatedAction
        : IAnimated
    {
        public string Name { get; }
        public LayeredAnimatedActionData LayerData { get; }
        [JsonIgnore]
        public IAnimation Animation { get; }

        [JsonConstructor]
        public LayeredAnimatedAction(string name,
            LayeredAnimatedActionData layerData)
        {
            Name = name;
            Animation = new StandardAnimation();
            LayerData = layerData;
        }

        public LayeredAnimatedAction(LayeredAnimatedAction copy)
        {
            Name = copy.Name;
            Animation = new StandardAnimation(copy.Animation);
            HunterCombatMR.AnimationKeyFrameManager.SyncFrames(Animation);
            LayerData = copy.LayerData;
        }

        public virtual void Initialize()
        {
            HunterCombatMR.AnimationKeyFrameManager.FillAnimationKeyFrames(Animation, LayerData.KeyFrameProfile, false, LayerData.Loop);
        }

        public void AddNewLayer(AnimationLayer layerInfo)
        {
            LayerData.Layers.Add(layerInfo);
        }

        public virtual void Update()
        {
            Animation.AdvanceFrame();
        }

        public void Pause()
        {
            if (Animation.IsPlaying)
                Animation.PauseAnimation();
        }

        public void Play()
        {
            if (!Animation.IsPlaying)
                Animation.StartAnimation();
        }

        public void Stop()
        {
            Pause();
            Animation.ResetAnimation(false);
        }

        public void Restart()
        {
            Animation.ResetAnimation(true);
        }

        public void UpdateKeyFrameLength(int keyFrameIndex, int frameAmount, bool setAmount = false, bool setDefault = false)
        {
            var isModified = LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex);

            if (setDefault)
            {
                setAmount = true;
                frameAmount = LayerData.KeyFrameProfile.DefaultKeyFrameSpeed;
            }

            HunterCombatMR.AnimationKeyFrameManager.AdjustKeyFrameLength(Animation,
                        keyFrameIndex,
                        frameAmount,
                        !setAmount);

            if (isModified)
            {
                LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.Remove(keyFrameIndex);

                if (!setDefault && Animation.KeyFrames[keyFrameIndex].FrameLength != LayerData.KeyFrameProfile.DefaultKeyFrameSpeed)
                {
                    LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.Add(keyFrameIndex, Animation.KeyFrames[keyFrameIndex].FrameLength);
                }
            } else if (Animation.KeyFrames[keyFrameIndex].FrameLength != LayerData.KeyFrameProfile.DefaultKeyFrameSpeed)
            {
                LayerData.KeyFrameProfile.SpecificKeyFrameSpeeds.Add(keyFrameIndex, Animation.KeyFrames[keyFrameIndex].FrameLength);
            } 
        }

        public void UpdateLoopType(LoopStyle newLoopType)
        {
            Animation.SetLoopMode(newLoopType);
            LayerData.Loop = newLoopType;
        }
    }
}