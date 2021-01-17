using HunterCombatMR.Enumerations;
using System.Collections.Generic;

namespace HunterCombatMR.Extensions
{
    public static class InputPermissionsExtensions
    {
        #region Private Fields

        private static readonly Dictionary<InputPermissions, char[]> InputMap = new Dictionary<InputPermissions, char[]>
        {
            { InputPermissions.FileUnsafeSymbols, new char[] { (char)32, '>', '<', ':', '"', '/', '\u005C', '|', '?', '*', '.'  } },
            { InputPermissions.FileSafeSymbols, new char[] { ',', '\'', '-', '!', '+', '_', '(', ')', '=', '[', ']' } },
            { InputPermissions.Capitals, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray() },
            { InputPermissions.LowerCases, "abcdefghijklmnopqrstuvwxyz".ToCharArray() },
            { InputPermissions.Numeric, "1234567890".ToCharArray() }
        };

        #endregion Private Fields

        #region Public Methods

        public static char[] GetAssociatedCharacters(this InputPermissions input)
            => (InputMap.ContainsKey(input)) ? InputMap[input] : new char[] { };

        #endregion Public Methods
    }
}