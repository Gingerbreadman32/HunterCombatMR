using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Events;
using HunterCombatMR.Extensions;

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
            action.AddKeyFrameEvent(new SetPlayerActionState(Enumerations.AttackState.ActiveAttack), activeFrame.ToFIndex());
            action.AddKeyFrameEvent(new SetPlayerActionState(Enumerations.AttackState.AttackRecovery), recoveryFrame.ToFIndex());

            return action;
        }

        internal static PlayerAction WithEvent(this PlayerAction action,
            Event<HunterCombatPlayer> @event,
            int frameNumber)
        {
            action.AddKeyFrameEvent(@event, frameNumber.ToFIndex());

            return action;
        }
    }
}