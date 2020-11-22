﻿using HunterCombatMR.Comparers;
using HunterCombatMR.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class LayerData
        : IEquatable<LayerData>
    {
        public KeyFrameProfile KeyFrameProfile { get; set; }

        public List<AnimationLayer> Layers { get; }

        public AnimationType ParentType { get; }

        public LoopStyle Loop { get; set; }
        
        [JsonConstructor]
        public LayerData(KeyFrameProfile frameProfile,
            IEnumerable<AnimationLayer> layers,
            AnimationType type,
            LoopStyle loop = 0)
        {
            KeyFrameProfile = frameProfile;
            Layers = new List<AnimationLayer>(layers);
            ParentType = type;
            Loop = loop;
        }

        public LayerData(LayerData copy)
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

        public bool Equals(LayerData other)
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