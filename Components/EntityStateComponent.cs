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
            StateVars = new float[256];
        }

        public GlobalStateController[] GlobalControllers { get; set; }

        [TriggerParameter(ComponentTriggerParams.StateVariables)]
        public float[] StateVars { get; set; }

        public StateInfo StateInfo { get; set; }

        [TriggerParameter(ComponentTriggerParams.StateNumber)]
        public int StateNumber { get => StateInfo.StateNumber; }

        [TriggerParameter(ComponentTriggerParams.StateTime)]
        public int StateTime { get => StateInfo.Time; }

        public EntityState GetCurrentState(Behavior behavior)
            => behavior.GetState(StateNumber);
    }
}