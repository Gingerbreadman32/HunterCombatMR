using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveCharacters(this string input, IEnumerable<char> characters)
            => new string(input.Where(x => !characters.Contains(x)).ToArray());
    }
}