using HunterCombatMR.AnimationEngine.Interfaces;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public class PlayerAttackAnimationInfo
        : IAnimated
    {
        public List<Dictionary<int, LayerFrameInfo>> Layers { get; }
        public IAnimation Animation { get; }

        public PlayerAttackAnimationInfo(IAnimation animation)
        {
            Animation = animation;
            Layers = new List<Dictionary<int, LayerFrameInfo>>();
        }

        public void InitializeLayer(Dictionary<int, LayerFrameInfo> layerInfo)
        {
            Layers.Add(layerInfo);
        }

        public void InitializeLayers(IEnumerable<Dictionary<int, LayerFrameInfo>> layers)
        {
            foreach (var layer in layers)
            {
                InitializeLayer(layer);
            }
        }

        public void Update()
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