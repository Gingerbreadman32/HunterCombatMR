using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Extensions
{
    public static class DictionaryExtensions
    {
        #region Public Methods

        public static SortedList<int, FrameLength> ConvertToLengthList(this IDictionary<int, int> keyValuePairs)
        {
            var list = new SortedList<int, FrameLength>();

            foreach (var pair in keyValuePairs)
            {
                list.Add(pair.Key, pair.Value.ToFLength());
            }

            return list;
        }

        #endregion Public Methods
    }
}