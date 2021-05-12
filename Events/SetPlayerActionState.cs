using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Events
{
    public sealed class SetPlayerActionState
        : Event<HunterCombatPlayer>
    {
        public SetPlayerActionState(AttackState state)
            : base()
        {
            ModifyParameter("ActionState", (float)state);
        }

        public override IEnumerable<EventParameter> DefaultParameters
        {
            get => new List<EventParameter>() { new EventParameter("ActionState", 0) };
        }

        public override void InvokeLogic(HunterCombatPlayer entity,
            Animator animator)
        {
            int parameterValue = (int)GetParameterValue ("ActionState");
            entity.StateController.ActionState = (AttackState)parameterValue;
        }
    }
}