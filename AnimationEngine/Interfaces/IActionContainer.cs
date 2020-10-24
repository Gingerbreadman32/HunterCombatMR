using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    /// <summary>
    /// Container that holds a list of action data to instantiate animations on mod load.
    /// </summary>
    public interface IActionContainer
    {
        /// <summary>
        /// The list of action data
        /// </summary>
        Dictionary<string, LayeredAnimatedActionData> AnimatedActions { get; }

        /// <summary>
        /// Override this method to create your list of actions
        /// </summary>
        void Load();
    }
}