using HunterCombatMR.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.State.Builders
{
    public static class StateSetChainMethods
    {
        public static StateSetBuilder WithAddedState(this StateSetBuilder builder,
            int stateNumber,
            EntityState state)
        {
            builder.AddState(stateNumber, state);
            return builder;
        }

        public static StateSetBuilder WithAddedStates(this StateSetBuilder builder,
            IEnumerable<KeyValuePair<int, EntityState>> states)
        {
            builder.AddStates(states);
            return builder;
        }

        public static StateSetBuilder WithState(this StateSetBuilder builder,
                    int stateNumber,
            EntityState state)
        {
            builder.SetState(stateNumber, state);
            return builder;
        }

        public static StateSetBuilder WithStates(this StateSetBuilder builder,
            IEnumerable<KeyValuePair<int, EntityState>> states)
        {
            builder.SetStates(states);
            return builder;
        }
    }

    public class StateSetBuilder
    {
        private Dictionary<int, EntityState> _states;
        private List<GlobalStateController> _globalControllers;

        public StateSetBuilder()
        {
            _states = new Dictionary<int, EntityState>();
            _globalControllers = new List<GlobalStateController>();
        }

        public StateSetBuilder(StateSet copy)
        {
            _states = copy.States.ToDictionary(x => x.Key, x => x.Value);
            _globalControllers = copy.GlobalStateControllers.ToList();
        }

        public IReadOnlyDictionary<int, EntityState> States { get => _states; }

        public void AddState(int stateNumber,
            EntityState state)
        {
            if (_states.ContainsKey(stateNumber))
                throw new Exception($"State list already contains a state with state no. of {stateNumber}");
            _states.Add(stateNumber, state);
        }

        public void AddGlobalController(string name,
            int priority,
            StateController controller)
        {
            _globalControllers.Add(new GlobalStateController(name, controller, priority));
        }

        public void RemoveGlobalController(string name)
        {
            if (HasGlobalController(name))
                _globalControllers.Remove(_globalControllers.Single(x => x.Name.Equals(name)));
        }

        public bool HasGlobalController(string name)
            => _globalControllers.Any(x => x.Name.Equals(name));

        public IEnumerable<GlobalStateController> ListGlobablControllersOfPriority(int priority)
            => _globalControllers.Where(x => x.Priority.Equals(priority)).ToList();

        public void AddStates(IEnumerable<KeyValuePair<int, EntityState>> states)
        {
            foreach (var state in states)
            {
                AddState(state.Key, state.Value);
            }
        }

        public StateSet Build()
        {
            if (!_states.Any())
                throw new Exception($"Stateset is empty. A stateset must have at least one state with a state no. of {StateNumberConstants.Default}!");

            return new StateSet(_states, _globalControllers);
        }

        public void RemoveState(int stateNumber)
        {
            if (!_states.ContainsKey(stateNumber))
                throw new Exception($"State with that state no. {stateNumber} does not exist in current set.");

            _states.Remove(stateNumber);
        }

        public void SetState(int stateNumber,
            EntityState state)
        {
            if (_states.ContainsKey(stateNumber))
            {
                _states[stateNumber] = state;
            }

            AddState(stateNumber, state);
        }

        public void SetStates(IEnumerable<KeyValuePair<int, EntityState>> states)
        {
            foreach (var state in states)
            {
                SetState(state.Key, state.Value);
            }
        }
    }
}