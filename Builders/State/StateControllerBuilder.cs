using HunterCombatMR.Constants;
using HunterCombatMR.Interfaces.State.Builders;
using HunterCombatMR.Models.State;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Builders.State
{
    public static class StateControllerBuilderChainMethods
    {
        public static StateControllerBuilder WithNewTrigger(this StateControllerBuilder builder,
            string parameter,
            string @operator,
            float value,
            int depth = 1)
        {
            builder.AddTrigger(new StateTrigger(parameter, @operator, value), depth);
            return builder;
        }

        public static StateControllerBuilder WithTrigger(this StateControllerBuilder builder,
            string triggerScript,
            int depth = 1)
        {
            builder.AddTrigger(new StateTrigger(triggerScript), depth);
            return builder;
        }

        public static StateControllerBuilder WithTrigger(this StateControllerBuilder builder,
            StateTrigger trigger,
            int depth = 1)
        {
            builder.AddTrigger(trigger, depth);
            return builder;
        }

        public static StateControllerBuilder WithTriggers(this StateControllerBuilder builder,
            IEnumerable<StateTrigger> triggers,
            int depth = 1)
        {
            builder.AddTriggers(triggers, depth);
            return builder;
        }

        public static StateControllerBuilder WithParameter(this StateControllerBuilder builder,
            object parameter)
        {
            if (parameter == null)
                throw new Exception("Parameter cannot be null!");

            builder.AddParameters(new object[1] { parameter });
            return builder;
        }

        public static StateControllerBuilder WithParameters(this StateControllerBuilder builder,
            IEnumerable<object> parameters)
        {
            if (parameters == null || !parameters.Any() || parameters.Any(x => x == null))
                throw new Exception("Parameters cannot be null or empty!");

            builder.AddParameters(parameters);
            return builder;
        }
    }

    public class StateControllerBuilder
        : IStateControllerBuilder
    {
        private const int MAX_TRIGGER_DEPTH = 6; // Maybe move this out of here

        private bool _ignoreHitPause;
        private List<object> _params;
        private int _persistency;
        private Dictionary<int, List<StateTrigger>> _triggers;
        private string _type;

        public StateControllerBuilder(string controllerType)
        {
            _type = controllerType;
            _persistency = 1;
            _ignoreHitPause = false;
            _triggers = new Dictionary<int, List<StateTrigger>>();
            _params = new List<object>();
        }

        public StateControllerBuilder(StateController copy)
        {
            _type = copy.Type;
            _persistency = copy.Persistency;
            _ignoreHitPause = copy.IgnoreHitPause;
            _triggers = ArrayUtils.JaggedArraytoDictionary(copy.Triggers).ToDictionary(x => x.Key, x => new List<StateTrigger>(x.Value));
            _params = new List<object>(copy.Parameters);
        }

        public string ControllerType { get => _type; set => _type = value; }
        public bool IgnoreHitPause { get => _ignoreHitPause; set => _ignoreHitPause = value; }
        public int Persistency { get => _persistency; set => _persistency = value; }
        public Dictionary<int, List<StateTrigger>> Triggers { get => _triggers; set => _triggers = value; }

        public void AddParameters(IEnumerable<object> parameters)
        {
            _params.AddRange(parameters);
        }

        public void AddTrigger(StateTrigger trigger,
                    int depth)
        {
            AddTriggers(new StateTrigger[1] { trigger }, depth);
        }

        public void AddTriggers(IEnumerable<StateTrigger> triggers,
                    int depth)
        {
            CheckTriggerDepth(depth);

            AddDepth(depth);

            _triggers[depth].AddRange(triggers);
        }

        public void RemoveTrigger(int depth,
            int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("Trigger cannot exist on an index lower than 0!");

            CheckTriggerDepth(depth);


        }

        private static void CheckTriggerDepth(int depth)
        {
            if (depth > MAX_TRIGGER_DEPTH || depth < 0)
                throw new Exception($"Trigger depth must be within 0 to {MAX_TRIGGER_DEPTH}.");
        }

        private void AddDepth(int depth)
        {
            if (!_triggers.ContainsKey(depth))
                _triggers.Add(depth, new List<StateTrigger>());
        }

        public StateController Build()
        {
            var triggers = ArrayUtils.DictionarytoJaggedArray(_triggers.ToDictionary(x => x.Key, x => (IEnumerable<StateTrigger>)x.Value));

            return new StateController(_type, _params.ToArray(), triggers, _persistency, _ignoreHitPause);
        }
    }
}