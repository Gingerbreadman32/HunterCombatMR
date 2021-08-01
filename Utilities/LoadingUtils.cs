using HunterCombatMR.Interfaces;
using System;

namespace HunterCombatMR.Utilities
{
    public static class LoadingUtils
    {
        public static T LoadCompact<T, Ta>(Ta[] arrayValue) where T : ICompact<Ta>
        => (T)Activator.CreateInstance(typeof(T), new object[] { arrayValue });
    }
}