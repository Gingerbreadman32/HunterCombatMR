using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    /// <inheritdoc/>
    public abstract class ActionContainer
    {
        public Dictionary<string, LayerData> AnimatedActions { get; }

        public ActionContainer()
        {
            AnimatedActions = new Dictionary<string, LayerData>();
        }

        public abstract void Load();
    }
}