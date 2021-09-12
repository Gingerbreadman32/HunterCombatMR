using HunterCombatMR.Extensions;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Behavior;
using HunterCombatMR.Models.State;
using HunterCombatMR.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Components
{
    public struct BehaviorComponent
    {
        private Behavior[] _behaviors;

        public BehaviorComponent(IEnumerable<Behavior> behaviors)
        {
            _behaviors = behaviors.ToArray();
        }

        public BehaviorComponent(Behavior behavior)
        {
            _behaviors = new Behavior[] { behavior };
        }

        public IReadOnlyDictionary<int, EntityState> ActiveStateList
        {
            get
            {
                var stateNumbers = Behaviors.Where(x => x.StateSet.HasValue).SelectMany(x => x.StateSet.Value.States.Keys).Distinct();
                return stateNumbers.SelectWhere<int, EntityState>(TryGetState).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Behavior[] Behaviors { get => _behaviors; }

        public bool TryGetState(int stateNumber, 
            out EntityState state)
        {
            state = default(EntityState);

            if (!_behaviors.Any(x => x.HasState(stateNumber)))
                return false;
            //throw new Exception($"No stateset on this entity contains a state with state no. {stateNumber}.");

            state = _behaviors.First(x => x.HasState(stateNumber)).GetState(stateNumber);

            return true;
        }

        public bool TryGetAnimation(int animationNumber, 
            out CustomAnimation animation)
        {
            CustomAnimation temp = default(CustomAnimation);
            var result = _behaviors.FirstOrDefault(x => x.TryGetAnimation(animationNumber, out temp));
            animation = temp;
            return result != null;
        }

        public IEnumerable<CustomAnimation> ListAnimations()
            => _behaviors.SelectMany(x => x.Animations.Values);

        public void AddBehavior(Behavior behavior)
        {
            if (_behaviors.Any(x => x.Name.Equals(behavior.Name)))
                throw new System.Exception("Behavior with that name already exists on this entity!");

            ArrayUtils.Add(ref _behaviors, behavior);
        }

        public void InsertBehavior(Behavior behavior,
            int index)
        {
            if (_behaviors.Any(x => x.Name.Equals(behavior.Name)))
                throw new System.Exception("Behavior with that name already exists on this entity!");

            ArrayUtils.AddOrInsert(ref _behaviors, behavior, index);
        }
    }
}