using HunterCombatMR.AnimationEngine.Interfaces;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class LayeredAnimatedAction
        : IAnimated
    {
        public string Name { get; }
        public LayeredAnimatedActionData LayerData { get; }
        public IAnimation Animation { get; }

        public LayeredAnimatedAction(string name,
            LayeredAnimatedActionData data)
        {
            Name = name;
            Animation = new StandardAnimation();
            LayerData = new LayeredAnimatedActionData(data);
        }

        public virtual void Initialize()
        {
            HunterCombatMR.AnimationKeyFrameManager.FillAnimationKeyFrames(Animation, LayerData.KeyFrameProfile, false);
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
                Animation.StopAnimation();
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
    }
}