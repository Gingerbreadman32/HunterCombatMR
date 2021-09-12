using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Input;
using HunterCombatMR.Models.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Behavior
{
    public class Behavior
    {
        public Behavior(string name)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException("Name must not be nothing!");
            Animations = new Dictionary<int, CustomAnimation>();
        }

        public Behavior(string name,
            StateSet states,
            IEnumerable<KeyValuePair<int, CustomAnimation>> animations,
            CommandList? commands)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException("Name must not be nothing!");
            StateSet = states;
            Animations = animations?.ToDictionary(x => x.Key, x => x.Value) ?? throw new ArgumentNullException("Animation list must not be null!");
            CommandList = commands;
        }

        public IDictionary<int, CustomAnimation> Animations { get; }
        public CommandList? CommandList { get; }
        public string Name { get; }
        public StateSet? StateSet { get; }

        // Sprites

        // Sounds

        public EntityState GetState(int stateNumber)
        {
            if (!HasState(stateNumber))
                throw new Exception($"No stateset on this entity contains a state with state no. {stateNumber}.");

            return StateSet.Value.States[stateNumber];
        }

        public bool HasState(int stateNumber)
            => StateSet?.States.ContainsKey(stateNumber) ?? false;

        public bool TryGetAnimation(int animationNumber, 
            out CustomAnimation animation)
            => Animations.TryGetValue(animationNumber, out animation);
    }
}