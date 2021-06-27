using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Events;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Action;

namespace HunterCombatMR.Seeds.Attacks
{
    internal static class PlayerAttackSeed
    {
        internal static CustomAction<HunterCombatPlayer> CreateDefault(string internalName,
            string displayName,
            int activeFrame = 0,
            int recoveryFrame = 0)
        {
            var action = new CustomAction<HunterCombatPlayer>(internalName, internalName, displayName);

            action.Animations.AddAnimation(internalName);

            action.Events.AddKeyframeEvent(new SetPlayerActionState(Enumerations.AttackState.AttackStartup), FrameIndex.Zero);
            action.Events.AddKeyframeEvent(new SetPlayerActionState(Enumerations.AttackState.ActiveAttack), activeFrame);
            action.Events.AddKeyframeEvent(new SetPlayerActionState(Enumerations.AttackState.AttackRecovery), recoveryFrame);

            return action;
        }

        internal static CustomAction<HunterCombatPlayer> WithEvent(this CustomAction<HunterCombatPlayer> action,
            Event<HunterCombatPlayer> @event,
            int frameNumber)
        {
            action.Events.AddKeyframeEvent(@event, frameNumber);

            return action;
        }
    }
}