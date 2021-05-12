using HunterCombatMR.Models;
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
            if (x.DefaultKeyFrameLength != y.DefaultKeyFrameLength)
                return false;
            if (x.KeyFrameAmount != y.KeyFrameAmount)
                return false;
            if (x.KeyFrameLengths.Count != y.KeyFrameLengths.Count)
                return false;

            foreach (int k in x.KeyFrameLengths.Keys)
            {
                if (!y.KeyFrameLengths.ContainsKey(k))
                    return false;

                if (!x.KeyFrameLengths[k].Equals(y.KeyFrameLengths[k]))
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