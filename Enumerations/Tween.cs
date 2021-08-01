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

    public enum TweenType
    {
        None = 0,
        Linear = 1,
        Ease = 2,
        Fade = 4,
    }
}