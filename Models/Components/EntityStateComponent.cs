using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
using HunterCombatMR.Models.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Components
{
    public struct EntityStateComponent
    {
        public EntityStateComponent(IEnumerable<StateSet> stateSets)
        {
            StateSets = stateSets.ToArray();
            CurrentStateNumber = 0;
            CurrentStateInfo = new StateInfo();
        }

        public EntityStateComponent(StateSet stateSet)
        {
            StateSets = new StateSet[] { stateSet };
            CurrentStateNumber = 0;
            CurrentStateInfo = new StateInfo();
        }

        public int CurrentStateNumber { get; set; }
        
        public StateInfo CurrentStateInfo { get; set; }

        [TriggerParameter(CommonTriggerParams.StateTime)]
        public int CurrentStateTime { get => CurrentStateInfo.Time; }

        public StateSet[] StateSets { get; set; }

        public EntityState GetCurrentState()
            => GetState(CurrentStateNumber);

        public EntityState GetState(int stateNumber)
        {
            if (!StateSets.Any(x => x.States.ContainsKey(stateNumber)))
                throw new Exception($"No stateset on this entity contains a state with state no. {stateNumber}.");

            return StateSets.First(x => x.States.ContainsKey(stateNumber)).States[stateNumber];
        }

        public void SetState(int stateNumber)
        {
            CurrentStateInfo = new StateInfo(Array.FindIndex(StateSets, x => x.States.ContainsKey(stateNumber)), CurrentStateNumber);
            CurrentStateNumber = stateNumber;
        }
    }
}