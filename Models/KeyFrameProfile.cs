using HunterCombatMR.Comparers;
using HunterCombatMR.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models
{
    public class KeyFrameProfile
        : IEquatable<KeyFrameProfile>
    {
        public KeyFrameProfile()
        {
            KeyFrameAmount = FrameLength.One;
            KeyFrameLengths = new SortedList<int, FrameLength>();
        }

        [JsonConstructor]
        public KeyFrameProfile(FrameLength keyFrameAmount,
            FrameLength defaultKeyFrameSpeed,
            SortedList<int, FrameLength> keyFrameSpeeds = null)
        {
            KeyFrameAmount = keyFrameAmount;
            DefaultKeyFrameLength = defaultKeyFrameSpeed;

            if (keyFrameSpeeds != null)
                KeyFrameLengths = new SortedList<int, FrameLength>(keyFrameSpeeds);
            else
                KeyFrameLengths = new SortedList<int, FrameLength>();
        }

        public KeyFrameProfile(SortedList<int, Keyframe> keyFrames,
            FrameLength defaultKeyFrameSpeed)
        {
            KeyFrameAmount = keyFrames.Count();
            DefaultKeyFrameLength = defaultKeyFrameSpeed;
            KeyFrameLengths = CreateTimingsFromKeyFrames(keyFrames);
        }

        public KeyFrameProfile(KeyFrameProfile copy)
        {
            KeyFrameAmount = copy.KeyFrameAmount;
            DefaultKeyFrameLength = copy.DefaultKeyFrameLength;

            if (copy.KeyFrameLengths != null && copy.KeyFrameLengths.Any())
                KeyFrameLengths = new SortedList<int, FrameLength>(copy.KeyFrameLengths);
            else
                KeyFrameLengths = new SortedList<int, FrameLength>();
        }

        public FrameLength DefaultKeyFrameLength { get; private set; } = FrameLength.One;
        public FrameLength KeyFrameAmount { get; set; }
        public SortedList<int, FrameLength> KeyFrameLengths { get; set; }

        public static KeyFrameProfile operator +(KeyFrameProfile left, KeyFrameProfile right)
        {
            var newProfile = new KeyFrameProfile(left);

            newProfile.KeyFrameAmount = left.KeyFrameAmount + right.KeyFrameAmount;

            for (var f = 0; f < right.KeyFrameAmount; f++)
            {
                if (right.KeyFrameLengths.ContainsKey(f))
                {
                    newProfile.KeyFrameLengths.Add(f + left.KeyFrameAmount, right.KeyFrameLengths[f]);
                    continue;
                }

                newProfile.KeyFrameLengths.Add(f + left.KeyFrameAmount, right.DefaultKeyFrameLength);
            }

            return newProfile;
        }

        public void Clear()
        {
            KeyFrameAmount = FrameLength.One;
            KeyFrameLengths.Clear();
            DefaultKeyFrameLength = FrameLength.One;
        }

        public bool Equals(KeyFrameProfile other)
        {
            KeyFrameProfileEqualityComparer comparer = new KeyFrameProfileEqualityComparer();

            return comparer.Equals(this, other);
        }

        public void RemoveKeyFrame(int keyFrameIndex)
        {
            if (KeyFrameLengths.ContainsKey(keyFrameIndex))
                KeyFrameLengths.Remove(keyFrameIndex);

            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        public void SwitchKeyFrames(int keyFrameIndex,
                    int newFrameIndex)
        {
            int? curSpeed = KeyFrameLengths.ContainsKey(keyFrameIndex) ? KeyFrameLengths[keyFrameIndex] : new int?();
            int? newSpeed = KeyFrameLengths.ContainsKey(newFrameIndex) ? KeyFrameLengths[newFrameIndex] : new int?();

            SwitchIndex(keyFrameIndex, newSpeed);
            SwitchIndex(newFrameIndex, curSpeed);
        }

        private SortedList<int, FrameLength> CreateTimingsFromKeyFrames(SortedList<int, Keyframe> keyFrames)
        {
            var timings = new SortedList<int, FrameLength>();
            foreach (var keyFrame in keyFrames)
            {
                if (!keyFrame.Value.FrameLength.Equals(DefaultKeyFrameLength))
                    timings.Add(keyFrame.Key, keyFrame.Value.FrameLength);
            }
            return timings;
        }

        private void InheritPreviousKeyFrameProperties(int keyFrameIndex)
        {
            int nextFrameIndex = keyFrameIndex + 1;
            if (KeyFrameLengths.ContainsKey(nextFrameIndex))
            {
                KeyFrameLengths.Add(keyFrameIndex, KeyFrameLengths[nextFrameIndex]);
                KeyFrameLengths.Remove(nextFrameIndex);
            }

            if (KeyFrameLengths.Any() && nextFrameIndex <= KeyFrameLengths.OrderBy(x => x.Key).Last().Key)
                InheritPreviousKeyFrameProperties(nextFrameIndex);
        }

        private void SwitchIndex(int keyFrameIndex,
            int? newFrameTime)
        {
            if (!newFrameTime.HasValue)
                KeyFrameLengths[keyFrameIndex] = (FrameLength)newFrameTime;
            else if (KeyFrameLengths.ContainsKey(keyFrameIndex))
                KeyFrameLengths.Remove(keyFrameIndex);
        }
    }
}