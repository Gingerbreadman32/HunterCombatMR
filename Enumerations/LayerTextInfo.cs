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
        DefaultDepth = 16,
        TextureFrameRectangle = 32,
        TextureFrames = 64,
        All = ~0
    }
}