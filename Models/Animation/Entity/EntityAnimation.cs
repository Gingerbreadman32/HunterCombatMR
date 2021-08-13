using HunterCombatMR.Enumerations;
using System.Linq;

namespace HunterCombatMR.Models.Animation
{
    public struct EntityAnimation
    {
        public EntityAnimation(string name,
            AnimationType category,
            AnimationLayers layers,
            LoopStyle loopStyle = LoopStyle.Once)
        {
            Name = name;
            Category = category;
            LoopStyle = loopStyle;
            Layers = layers;
        }

        public AnimationType Category { get; }

        public AnimationLayers Layers { get; }
        public LoopStyle LoopStyle { get; }
        public string Name { get; }

        public int TotalFrames { get => Layers.FrameData.Sum(x => x.Frames); }
    }
}