using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string input, string contained)
            => input.ToLower().Contains(contained.ToLower());

        public static bool EqualsIgnoreCase(this string input, string contained)
            => input.ToLower().Equals(contained.ToLower());

        public static string RemoveCharacters(this string input, IEnumerable<char> characters)
            => new string(input.Where(x => !characters.Contains(x)).ToArray());
    }
}