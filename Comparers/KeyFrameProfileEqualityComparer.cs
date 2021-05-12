using HunterCombatMR.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Comparers
{
    internal class FrameEqualityComparer
        : IEqualityComparer<Dictionary<int, LayerFrameInfo>>
    {
        public bool Equals(Dictionary<int, LayerFrameInfo> x, Dictionary<int, LayerFrameInfo> y)
        {
            // early-exit checks
            if (null == y)
                return null == x;
            if (null == x)
                return false;
            if (x.Count != y.Count)
                return false;

            foreach (int k in x.Keys)
            {
                if (!y.ContainsKey(k))
                    return false;

                if (!x[k].Equals(y[k]))
                    return false;
            }

            return true;
        }

        public int GetHashCode(Dictionary<int, LayerFrameInfo> obj)
        {
            return obj.Values.GetHashCode(); // lol
        }
    }
}