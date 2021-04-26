using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Events;

namespace HunterCombatMR.Seeds.Attacks
{
    internal static class AttackSeed
    {
        internal static PlayerAction CreateDefault(string internalName,
            string displayName)
        {
            var action = new PlayerAction(internalName, displayName);

            action.Animations.AnimationReferences.Add(0, internalName);

            action.AddKeyFrameEvent(new SetPlayerActionState(Enumerations.AttackState.AttackStartup), FrameIndex.Zero);

            return action;
        }
    }
}