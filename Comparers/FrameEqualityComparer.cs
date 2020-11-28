using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Comparers
{
    internal class KeyFrameProfileEqualityComparer
        : IEqualityComparer<KeyFrameProfile>
    {
        public bool Equals(KeyFrameProfile x, KeyFrameProfile y)
        {
            // early-exit checks
            if (null == y)
                return null == x;
            if (null == x)
                return false;
            if (x.DefaultKeyFrameSpeed != y.DefaultKeyFrameSpeed)
                return false;
            if (x.KeyFrameAmount != y.KeyFrameAmount)
                return false;
            if (x.SpecificKeyFrameSpeeds.Count != y.SpecificKeyFrameSpeeds.Count)
                return false;

            foreach (int k in x.SpecificKeyFrameSpeeds.Keys)
            {
                if (!y.SpecificKeyFrameSpeeds.ContainsKey(k))
                    return false;

                if (!x.SpecificKeyFrameSpeeds[k].Equals(y.SpecificKeyFrameSpeeds[k]))
                    return false;
            }

            return true;
        }

        public int GetHashCode(KeyFrameProfile obj)
        {
            return obj.GetHashCode(); // lol
        }
    }
}