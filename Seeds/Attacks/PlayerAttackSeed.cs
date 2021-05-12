using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Events;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models;

namespace HunterCombatMR.Seeds.Attacks
{
    internal static class PlayerAttackSeed
    {
        internal static PlayerAction CreateDefault(string internalName,
            string displayName,
            int activeFrame = 0,
            int recoveryFrame = 0)
        {
            var action = new PlayerAction(internalName, displayName);

            action.Animations.AnimationReferences.Add(0, internalName);

            action.AddKeyFrameEvent(new SetPlayerActionState(Enumerations.AttackState.AttackStartup), FrameIndex.Zero);
            action.AddKeyFrameEvent(new SetPlayerActionState(Enumerations.AttackState.ActiveAttack), activeFrame);
            action.AddKeyFrameEvent(new SetPlayerActionState(Enumerations.AttackState.AttackRecovery), recoveryFrame);

            return action;
        }

        internal static PlayerAction WithEvent(this PlayerAction action,
            Event<HunterCombatPlayer> @event,
            int frameNumber)
        {
            action.AddKeyFrameEvent(@event, frameNumber);

            return action;
        }
    }
}