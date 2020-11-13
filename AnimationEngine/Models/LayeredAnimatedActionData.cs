using HunterCombatMR.Comparers;
using HunterCombatMR.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class LayeredAnimatedActionData
        : IEquatable<LayeredAnimatedActionData>
    {
        public KeyFrameProfile KeyFrameProfile { get; set; }

        public List<AnimationLayer> Layers { get; }

        public AnimationType ParentType { get; }

        public LoopStyle Loop { get; set; }
        
        [JsonConstructor]
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
            Layers = new List<AnimationLayer>();
            ParentType = copy.ParentType;
            Loop = copy.Loop;

            foreach (AnimationLayer layer in copy.Layers)
            {
                Layers.Add(new AnimationLayer(layer));
            }
        }

        public bool Equals(LayeredAnimatedActionData other)
        {
            bool paramEquals = KeyFrameProfile.Equals(other.KeyFrameProfile)
                && ParentType.Equals(other.ParentType)
                && Loop.Equals(other.Loop);
            LayerEqualityComparer comparer = new LayerEqualityComparer();

            bool listEquals = comparer.Equals(Layers, other.Layers);

            return paramEquals && listEquals;
        }
    }
}