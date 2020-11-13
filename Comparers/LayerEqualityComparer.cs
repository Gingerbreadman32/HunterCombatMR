using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Comparers
{
    internal class LayerEqualityComparer
        : IEqualityComparer<List<AnimationLayer>>
    {
        public bool Equals(List<AnimationLayer> x, List<AnimationLayer> y)
        {
            // early-exit checks
            if (null == y)
                return null == x;
            if (null == x)
                return false;
            if (x.Count != y.Count)
                return false;

            foreach (AnimationLayer k in x)
            {
                if (!k.Equals(y[x.IndexOf(k)]))
                    return false;
            }

            return true;
        }

        public int GetHashCode(List<AnimationLayer> obj)
        {
            return obj.GetHashCode(); // lol
        }
    }
}