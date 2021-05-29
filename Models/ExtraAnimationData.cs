using HunterCombatMR.Comparers;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models
{
    public sealed class ExtraAnimationData
        : IEquatable<ExtraAnimationData>
    {
        [JsonConstructor]
        public ExtraAnimationData(KeyFrameProfile frameProfile,
            IEnumerable<AnimationLayer> layers,
            LoopStyle loop = 0)
        {
            KeyFrameProfile = frameProfile;
            Layers = new List<AnimationLayer>(layers);
            Loop = loop;
        }

        public ExtraAnimationData(ExtraAnimationData copy)
        {
            KeyFrameProfile = new KeyFrameProfile(copy.KeyFrameProfile);
            Layers = new List<AnimationLayer>();
            Loop = copy.Loop;

            foreach (AnimationLayer layer in copy.Layers)
            {
                Layers.Add(new AnimationLayer(layer));
            }
        }

        public KeyFrameProfile KeyFrameProfile { get; set; }

        public List<AnimationLayer> Layers { get; }

        public LoopStyle Loop { get; set; }

        public void AddNewLayer(AnimationLayer layerInfo)
        {
            Layers.Add(layerInfo);
        }

        public bool Equals(ExtraAnimationData other)
        {
            bool paramEquals = KeyFrameProfile.Equals(other.KeyFrameProfile)
                && Loop.Equals(other.Loop);
            LayerEqualityComparer comparer = new LayerEqualityComparer();

            bool listEquals = comparer.Equals(Layers, other.Layers);

            return paramEquals && listEquals;
        }

        public IDictionary<AnimationLayer, LayerFrameInfo> GetFrameInfoForLayers(int keyFrameIndex)
            => Layers.ToDictionary(x => x, x => x.KeyFrames[keyFrameIndex]);

        public AnimationLayer GetLayer(string layerName)
                    => Layers.FirstOrDefault(x => x.DisplayName.Equals(layerName));

        public int GetTextureFrameAtKeyFrameForLayer(int keyFrameIndex,
                    string layerName)
            => Layers.SingleOrDefault(x => x.DisplayName.Equals(layerName))?.GetTextureFrame(keyFrameIndex) ?? 0;
    }
}