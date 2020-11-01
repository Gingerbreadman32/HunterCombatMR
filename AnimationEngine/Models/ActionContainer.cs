using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    /// <inheritdoc/>
    public abstract class ActionContainer
    {
        public Dictionary<string, LayeredAnimatedActionData> AnimatedActions { get; }

        public ActionContainer()
        {
            AnimatedActions = new Dictionary<string, LayeredAnimatedActionData>();
        }

        public abstract void Load();
    }
}