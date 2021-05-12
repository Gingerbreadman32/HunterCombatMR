using HunterCombatMR.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Seeds.Animations
{
    /// <inheritdoc/>
    internal abstract class AnimationSeed
    {
        internal Dictionary<string, LayerData> AnimatedActions { get; }

        internal AnimationSeed()
        {
            AnimatedActions = new Dictionary<string, LayerData>();
        }

        internal abstract void Load();
    }
}