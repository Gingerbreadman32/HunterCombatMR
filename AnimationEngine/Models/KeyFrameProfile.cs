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
        #region Public Constructors

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

        public KeyFrameProfile(int keyFrameAmount,
            int defaultKeyFrameSpeed,
            int[] keyFrameSpeeds)
        {
            KeyFrameAmount = keyFrameAmount;
            DefaultKeyFrameSpeed = defaultKeyFrameSpeed;
            SpecificKeyFrameSpeeds = keyFrameSpeeds.ToDictionary(x => x, x => keyFrameSpeeds[x]);
        }

        public KeyFrameProfile()
        {
            KeyFrameAmount = 0;
            DefaultKeyFrameSpeed = 1;
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

        #endregion Public Constructors

        #region Public Properties

        public int DefaultKeyFrameSpeed { get; set; }
        public int KeyFrameAmount { get; set; }
        public IDictionary<int, int> SpecificKeyFrameSpeeds { get; set; }

        #endregion Public Properties

        #region Public Methods

        public bool Equals(KeyFrameProfile other)
        {
            KeyFrameProfileEqualityComparer comparer = new KeyFrameProfileEqualityComparer();

            return comparer.Equals(this, other);
        }

        public void SwitchKeyFrames(int keyFrameIndex,
            int newFrameIndex)
        {
            int newCurrentFrame = 0;
            int newReplacedFrame = 0;

            if (SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex))
                newReplacedFrame = SpecificKeyFrameSpeeds[keyFrameIndex];

            if (SpecificKeyFrameSpeeds.ContainsKey(newFrameIndex))
                newCurrentFrame = SpecificKeyFrameSpeeds[newFrameIndex];

            SwitchIndex(keyFrameIndex, newCurrentFrame, (newCurrentFrame <= 0));
            SwitchIndex(newFrameIndex, newReplacedFrame, (newReplacedFrame <= 0));
        }

        public void RemoveKeyFrame(int keyFrameIndex)
        {
            if (SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex))
                SpecificKeyFrameSpeeds.Remove(keyFrameIndex);

            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        #endregion Public Methods

        #region Private Methods

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
            int newFrameTime,
            bool defaultSpeed)
        {
            if (!defaultSpeed)
                SpecificKeyFrameSpeeds[keyFrameIndex] = newFrameTime;
            else if (SpecificKeyFrameSpeeds.ContainsKey(keyFrameIndex))
                SpecificKeyFrameSpeeds.Remove(keyFrameIndex);
        }

        #endregion Private Methods
    }
}