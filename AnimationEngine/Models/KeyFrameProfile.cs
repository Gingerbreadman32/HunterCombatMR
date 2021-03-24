using HunterCombatMR.Comparers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class KeyFrameProfile
        : IEquatable<KeyFrameProfile>
    {
        #region Private Fields

        [JsonIgnore]
        private FrameLength _defaultFrameLength;

        #endregion Private Fields

        #region Public Constructors

        public KeyFrameProfile()
        {
            KeyFrameAmount = 0;
            DefaultKeyFrameSpeed = 1;
            SpecificKeyFrameSpeeds = new SortedList<int, FrameLength>();
        }

        [JsonConstructor]
        public KeyFrameProfile(int keyFrameAmount,
            int defaultKeyFrameSpeed,
            SortedList<int, FrameLength> keyFrameSpeeds = null)
        {
            KeyFrameAmount = keyFrameAmount;
            DefaultKeyFrameSpeed = defaultKeyFrameSpeed;

            if (keyFrameSpeeds != null)
                SpecificKeyFrameSpeeds = new SortedList<int, FrameLength>(keyFrameSpeeds);
            else
                SpecificKeyFrameSpeeds = new SortedList<int, FrameLength>();
        }

        public KeyFrameProfile(SortedList<int, KeyFrame> keyFrames,
            int defaultKeyFrameSpeed)
        {
            KeyFrameAmount = keyFrames.Count();
            DefaultKeyFrameSpeed = defaultKeyFrameSpeed;
            SpecificKeyFrameSpeeds = CreateTimingsFromKeyFrames(keyFrames);
        }

        public KeyFrameProfile(KeyFrameProfile copy)
        {
            KeyFrameAmount = copy.KeyFrameAmount;
            DefaultKeyFrameSpeed = copy.DefaultKeyFrameSpeed;

            if (copy.SpecificKeyFrameSpeeds != null && copy.SpecificKeyFrameSpeeds.Any())
                SpecificKeyFrameSpeeds = new SortedList<int, FrameLength>(copy.SpecificKeyFrameSpeeds);
            else
                SpecificKeyFrameSpeeds = new SortedList<int, FrameLength>();
        }

        #endregion Public Constructors

        #region Public Properties

        public int DefaultKeyFrameSpeed { get => _defaultFrameLength; set => _defaultFrameLength = (FrameLength)value; }
        [JsonIgnore]
        public FrameLength DefaultKeyFrameLength { get => _defaultFrameLength; }
        public int KeyFrameAmount { get; set; }
        public SortedList<int, FrameLength> SpecificKeyFrameSpeeds { get; set; }

        #endregion Public Properties

        #region Public Methods

        public bool Equals(KeyFrameProfile other)
        {
            KeyFrameProfileEqualityComparer comparer = new KeyFrameProfileEqualityComparer();

            return comparer.Equals(this, other);
        }

        public void RemoveKeyFrame(int keyFrameIndex)
        {
            if (SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex))
                SpecificKeyFrameSpeeds.Remove(keyFrameIndex);

            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        public void SwitchKeyFrames(int keyFrameIndex,
                    int newFrameIndex)
        {
            int? curSpeed = SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex) ? SpecificKeyFrameSpeeds[keyFrameIndex] : new int?();
            int? newSpeed = SpecificKeyFrameSpeeds.ContainsKey(newFrameIndex) ? SpecificKeyFrameSpeeds[newFrameIndex] : new int?();

            SwitchIndex(keyFrameIndex, newSpeed);
            SwitchIndex(newFrameIndex, curSpeed);
        }

        #endregion Public Methods

        #region Private Methods

        private SortedList<int, FrameLength> CreateTimingsFromKeyFrames(SortedList<int, KeyFrame> keyFrames)
        {
            var timings = new SortedList<int, FrameLength>();
            foreach (var keyFrame in keyFrames)
            {
                if (!keyFrame.Value.FrameLength.Equals(DefaultKeyFrameSpeed))
                    timings.Add(keyFrame.Key, keyFrame.Value.FrameLength);
            }
            return timings;
        }

        private void InheritPreviousKeyFrameProperties(int keyFrameIndex)
        {
            int nextFrameIndex = keyFrameIndex + 1;
            if (SpecificKeyFrameSpeeds.ContainsKey(nextFrameIndex))
            {
                SpecificKeyFrameSpeeds.Add(keyFrameIndex, SpecificKeyFrameSpeeds[nextFrameIndex]);
                SpecificKeyFrameSpeeds.Remove(nextFrameIndex);
            }

            if (SpecificKeyFrameSpeeds.Any() && nextFrameIndex <= SpecificKeyFrameSpeeds.OrderBy(x => x.Key).Last().Key)
                InheritPreviousKeyFrameProperties(nextFrameIndex);
        }

        private void SwitchIndex(int keyFrameIndex,
            int? newFrameTime)
        {
            if (!newFrameTime.HasValue)
                SpecificKeyFrameSpeeds[keyFrameIndex] = (FrameLength)newFrameTime;
            else if (SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex))
                SpecificKeyFrameSpeeds.Remove(keyFrameIndex);
        }

        #endregion Private Methods
    }
}