﻿using HunterCombatMR.Comparers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class KeyFrameProfile
        : IEquatable<KeyFrameProfile>
    {
        public int KeyFrameAmount { get; set; }
        public int DefaultKeyFrameSpeed { get; set; }
        public IDictionary<int, int> SpecificKeyFrameSpeeds { get; set; }

        [JsonConstructor]
        public KeyFrameProfile(int keyFrameAmount,
            int defaultKeyFrameSpeed,
            IDictionary<int, int> keyFrameSpeeds = null)
        {
            KeyFrameAmount = keyFrameAmount;
            DefaultKeyFrameSpeed = defaultKeyFrameSpeed;

            if (keyFrameSpeeds != null)
                SpecificKeyFrameSpeeds = new Dictionary<int, int>(keyFrameSpeeds);
            else
                SpecificKeyFrameSpeeds = new Dictionary<int, int>();
        }

        public KeyFrameProfile(KeyFrameProfile copy)
        {
            KeyFrameAmount = copy.KeyFrameAmount;
            DefaultKeyFrameSpeed = copy.DefaultKeyFrameSpeed;

            if (copy.SpecificKeyFrameSpeeds != null && copy.SpecificKeyFrameSpeeds.Any())
                SpecificKeyFrameSpeeds = new Dictionary<int, int>(copy.SpecificKeyFrameSpeeds);
            else
                SpecificKeyFrameSpeeds = new Dictionary<int, int>();
        }

        public bool Equals(KeyFrameProfile other)
        {
            KeyFrameProfileEqualityComparer comparer = new KeyFrameProfileEqualityComparer();

            return comparer.Equals(this, other);
        }
    }
}