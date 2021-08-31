using System;

namespace HunterCombatMR.Enumerations
{
    [Flags]
    public enum TweenTargets
    {
        None = 0,
        Position = 1,
        Rotation = 2,
        Alpha = 3
    }

    public enum TweenProperties
    {
        None = 0,
        EaseIn = 1,
        EaseOut = 2,
    }
}