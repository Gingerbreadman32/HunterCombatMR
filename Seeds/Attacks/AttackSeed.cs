using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Utilities;

namespace HunterCombatMR.Seeds.Attacks
{
    internal static class AttackSeed
    {
        internal static PlayerAction CreateDefault(string internalName,
            string displayName)
        {
            var action = new PlayerAction(internalName, displayName);

            action.Animations.AnimationReferences.Add(0, internalName);

            return action;
        }
    }
}