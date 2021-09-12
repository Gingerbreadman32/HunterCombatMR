using HunterCombatMR.Enumerations;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Animation
{
    public struct CustomAnimation
    {
        public CustomAnimation(string name,
            AnimationType category,
            IEnumerable<Layer> layers,
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

        public Layer[] Layers { get; }

        public FrameLength[] FrameData { get; }
        public LoopStyle LoopStyle { get; }
        public string Name { get; }

        public int TotalFrames { get => FrameData?.Sum(x => x) ?? 0; }
    }
}