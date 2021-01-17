using System;

namespace HunterCombatMR.Enumerations
{
    [Flags]
    public enum InputPermissions
    {
        None = 0,
        Capitals = 1,
        LowerCases = 2,
        Alphabet = 3,
        Numeric = 8,
        Alphanumeric = InputPermissions.Alphabet | InputPermissions.Numeric,
        FileUnsafeSymbols = 16,
        FileSafeSymbols = 32,
        FileSafe = InputPermissions.Alphanumeric | InputPermissions.FileSafeSymbols,
        Any = ~0
    }
}