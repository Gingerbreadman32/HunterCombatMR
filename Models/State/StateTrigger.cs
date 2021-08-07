using HunterCombatMR.Utilities;
using System;

namespace HunterCombatMR.Models.State
{
    public struct StateTrigger
    {
        public StateTrigger(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                throw new Exception("Trigger must have a script to convert!");

            var split = script.Split(' ');
            Validate(split[0], split[1], false);

            float value;

            if (!float.TryParse(split[2], out value))
                throw new Exception("Must have a valid float value!");

            Parameter = split[0];
            Operator = split[1];
            Value = value;
        }

        public StateTrigger(string parameter,
            string @operator,
            float value)
        {
            Validate(parameter, @operator, false);

            Parameter = parameter;
            Operator = @operator;
            Value = value;
        }

        public string Operator { get; }
        public string Parameter { get; }
        public float Value { get; }

        public static bool Validate(string parameter,
            string @operator,
            bool ignoreErrors = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parameter))
                    throw new Exception("Parameter must be not be nothing!");

                if (string.IsNullOrWhiteSpace(@operator) || !OperatorUtils.IsValidComparisonOperator(@operator))
                    throw new Exception("Operator must be a valid comparison operator! (Ex. >, <, <=, >=, =, !=, is, not)");

                return true;
            }
            catch (Exception ex)
            {
                if (ignoreErrors)
                    return false;

                throw ex;
            }
        }

        public string PrintScript()
                    => $"{Parameter} {Operator} {Value}";
    }
}