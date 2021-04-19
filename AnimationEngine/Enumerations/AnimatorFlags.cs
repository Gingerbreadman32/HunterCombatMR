using System;

namespace HunterCombatMR.AnimationEngine.Enumerations
{
    [Flags]
    public enum AnimatorFlags
    {
        None = 0,
        Started = 1,
        Locked = 2,
        Paused = 3,
        Reversed = 4,
        PlayingReversed = 5
    }
}