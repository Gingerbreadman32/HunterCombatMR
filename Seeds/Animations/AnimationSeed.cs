using HunterCombatMR.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Seeds.Animations
{
    /// <inheritdoc/>
    internal abstract class AnimationSeed
    {
        internal Dictionary<string, ExtraAnimationData> AnimatedActions { get; }

        internal AnimationSeed()
        {
            AnimatedActions = new Dictionary<string, ExtraAnimationData>();
        }

        internal abstract void Load();
    }
}