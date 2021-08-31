using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Components
{
    public struct EntityStateComponent
    {
        public EntityStateComponent(IEnumerable<StateSet> stateSets,
            IEnumerable<CustomAnimation> animationSet)
        {
            StateSets = stateSets.ToArray();
            CurrentStateNumber = 0;
            CurrentStateInfo = new StateInfo();
            AnimationSet = animationSet.ToArray();
        }

        public EntityStateComponent(StateSet stateSet,
            IEnumerable<CustomAnimation> animationSet)
        {
            StateSets = new StateSet[] { stateSet };
            CurrentStateNumber = 0;
            CurrentStateInfo = new StateInfo();
            AnimationSet = animationSet.ToArray();
        }

        public int CurrentStateNumber { get; set; }
        
        public StateInfo CurrentStateInfo { get; set; }

        [TriggerParameter(CommonTriggerParams.StateTime)]
        public int CurrentStateTime { get => CurrentStateInfo.Time; }

        public StateSet[] StateSets { get; set; }

        public CustomAnimation[] AnimationSet { get; set; }

        public EntityState GetCurrentState()
            => GetState(CurrentStateNumber);

        public EntityState GetState(int stateNumber)
        {
            if (!StateSets.Any(x => x.States.ContainsKey(stateNumber)))
                throw new Exception($"No stateset on this entity contains a state with state no. {stateNumber}.");

            return StateSets.First(x => x.States.ContainsKey(stateNumber)).States[stateNumber];
        }
    }
}