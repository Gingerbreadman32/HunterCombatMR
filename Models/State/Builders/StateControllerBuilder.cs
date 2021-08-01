using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces.State.Builders;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.State.Builders
{
    public static class StateControllerBuilderChainMethods
    {
        public static StateControllerBuilder WithBasicTrigger(this StateControllerBuilder builder,
            string parameter,
            string @operator,
            IComparable value,
            int depth = 1)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                throw new Exception("Parameter must be not be nothing!");

            if (string.IsNullOrWhiteSpace(@operator) || !OperatorUtils.IsValidComparisonOperator(@operator))
                throw new Exception("Operator must be a valid comparison operator! (Ex. >, <, <=, >=, =, !=, is, not)");

            if (value == null)
                throw new Exception("Must have a valid value!");

            string script = $"{parameter} {@operator} {value}";

            builder.AddTrigger(new StateTrigger(script), depth);
            return builder;
        }

        public static StateControllerBuilder WithTrigger(this StateControllerBuilder builder,
            string trigger,
            int depth = 1)
        {
            if (string.IsNullOrWhiteSpace(trigger))
                throw new Exception("Trigger must be not be nothing!");

            builder.AddTrigger(new StateTrigger(trigger), depth);
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
        private const int MAX_TRIGGER_DEPTH = 6;

        private bool _ignoreHitPause;
        private List<object> _params;
        private int _persistency;
        private IDictionary<int, StateTrigger> _triggers;
        private StateControllerType _type;

        public StateControllerBuilder(StateControllerType controllerType)
        {
            _type = controllerType;
            _persistency = 1;
            _ignoreHitPause = false;
            _triggers = new Dictionary<int, StateTrigger>();
            _params = new List<object>();
        }

        public StateControllerType ControllerType { get => _type; set => _type = value; }
        public bool IgnoreHitPause { get => _ignoreHitPause; set => _ignoreHitPause = value; }
        public int Persistency { get => _persistency; set => _persistency = value; }

        public void AddParameters(IEnumerable<object> parameters)
        {
            _params.AddRange(parameters);
        }

        public void AddTrigger(StateTrigger trigger,
                    int depth)
        {
            if (depth > MAX_TRIGGER_DEPTH || depth < 0)
                throw new Exception($"Trigger depth must be within 0 to {MAX_TRIGGER_DEPTH}.");

            _triggers.Add(depth, trigger);
        }

        public StateController Build()
        {
            var triggers = new StateTrigger[][] { };
            ArrayUtils.ResizeAndFillArray(ref triggers, _triggers.Keys.Distinct().Count() + 1, new StateTrigger[0]);

            foreach (var row in _triggers.Keys.Distinct())
            {
                triggers[row] = _triggers.Where(x => x.Key.Equals(row)).Select(x => x.Value).ToArray();
            }

            return new StateController(_type, _params.ToArray(), triggers, _persistency, _ignoreHitPause);
        }
    }
}