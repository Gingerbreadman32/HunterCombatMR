using HunterCombatMR.Enumerations;
using HunterCombatMR.Models.Animation.Entity;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Animation
{
    public struct EntityAnimation
    {
        public EntityAnimation(string name,
            AnimationType category,
            IEnumerable<EntityAnimationLayer> layers,
            LoopStyle loopStyle,
            IEnumerable<FrameLength> frameData)
        {
            Name = name;
            Category = category;
            LoopStyle = loopStyle;
            Layers = layers.ToArray();
            FrameData = frameData.ToArray();
        }

        public AnimationType Category { get; }

        public EntityAnimationLayer[] Layers { get; }

        public FrameLength[] FrameData { get; }
        public LoopStyle LoopStyle { get; }
        public string Name { get; }

        public int TotalFrames { get => FrameData.Sum(x => x); }
    }
}