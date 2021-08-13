using System;

namespace HunterCombatMR.Models.Animation.Entity
{
    [Flags]
    public enum AnimationFlags
    {
        None = 0,
        Paused = 1,
        Reversed = 2
    }
}