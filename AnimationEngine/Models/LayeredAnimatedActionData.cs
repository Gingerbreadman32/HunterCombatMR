using HunterCombatMR.Enumerations;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class LayeredAnimatedActionData
    {
        public KeyFrameProfile KeyFrameProfile { get; set; }

        public List<AnimationLayer> Layers { get; }

        public AnimationType ParentType { get; }

        public LoopStyle Loop { get; set; }

        public LayeredAnimatedActionData(KeyFrameProfile frameProfile,
            IEnumerable<AnimationLayer> layers,
            AnimationType type,
            LoopStyle loop = 0)
        {
            KeyFrameProfile = frameProfile;
            Layers = new List<AnimationLayer>(layers);
            ParentType = type;
            Loop = loop;
        }

        public LayeredAnimatedActionData(LayeredAnimatedActionData copy)
        {
            KeyFrameProfile = copy.KeyFrameProfile;
            Layers = new List<AnimationLayer>(copy.Layers);
            ParentType = copy.ParentType;
            Loop = copy.Loop;
        }
    }
}