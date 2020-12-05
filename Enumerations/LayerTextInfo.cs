using System;

namespace HunterCombatMR.Enumerations
{
    [Flags]
    internal enum LayerTextInfo
    {
        None = 0,
        Coordinates = 1,
        Rotation = 2,
        Orientation = 4,
        TextureName = 8,
        Depth = 16,
        Scale = 32
    }
}