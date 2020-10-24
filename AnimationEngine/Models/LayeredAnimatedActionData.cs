using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class LayeredAnimatedActionData
    {
        public KeyFrameProfile KeyFrameProfile { get; set; }

        public List<AnimationLayer> Layers { get; }

        public LayeredAnimatedActionData(KeyFrameProfile frameProfile,
            IEnumerable<AnimationLayer> layers)
        {
            KeyFrameProfile = frameProfile;
            Layers = new List<AnimationLayer>(layers);
        }

        public LayeredAnimatedActionData(LayeredAnimatedActionData copy)
        {
            KeyFrameProfile = copy.KeyFrameProfile;
            Layers = new List<AnimationLayer>(copy.Layers);
        }
    }
}