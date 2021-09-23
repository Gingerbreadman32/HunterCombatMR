using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
using HunterCombatMR.Models.Behavior;
using HunterCombatMR.Models.State;

namespace HunterCombatMR.Components
{
    public struct EntityStateComponent
    {
        public EntityStateComponent(StateInfo stateInfo,
            GlobalStateController[] globalControllers)
        {
            StateInfo = stateInfo;
            GlobalControllers = globalControllers;
        }

        public GlobalStateController[] GlobalControllers { get; set; }

        public StateInfo StateInfo { get; set; }

        [TriggerParameter(CommonTriggerParams.StateNumber)]
        public int StateNumber { get => StateInfo.StateNumber; }

        [TriggerParameter(CommonTriggerParams.StateTime)]
        public int StateTime { get => StateInfo.Time; }

        public EntityState GetCurrentState(Behavior behavior)
            => behavior.GetState(StateNumber);
    }
}