using HunterCombatMR.Models.State;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Components
{
    public struct EntityStateComponent
    {
        public EntityStateComponent(IEnumerable<StateSet> stateSets)
        {
            StateSets = stateSets.ToArray();
            CurrentState = 0;
            CurrentStateSet = 0;
            CurrentStateTime = 0;
        }

        public EntityStateComponent(StateSet stateSet)
        {
            StateSets = new StateSet[] { stateSet };
            CurrentState = 0;
            CurrentStateSet = 0;
            CurrentStateTime = 0;
        }

        public int CurrentState { get; set; }
        public int CurrentStateSet { get; set; }

        public int CurrentStateTime { get; set; }
        public StateSet[] StateSets { get; set; }
    }
}