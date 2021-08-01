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

            Script = script;
            Logic = Convert(Script);
        }

        public StateTrigger(string parameter,
            string @operator,
            object value)
        {
            ValidateBasicTrigger(parameter, @operator, value, false);

            Script = $"{parameter} {@operator} {value}";
            Logic = Convert(Script);
        }

        public Func<int, int, bool> Logic { get; }

        public string Script { get; }

        private static Func<int, int, bool> Convert(string text)
        {
            // Fill this
            return (x, y) => { return false; };
        }

        private static bool ValidateBasicTrigger(string parameter,
                                    string @operator,
            object value,
            bool ignoreErrors = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parameter))
                    throw new Exception("Parameter must be not be nothing!");

                if (string.IsNullOrWhiteSpace(@operator) || !OperatorUtils.IsValidComparisonOperator(@operator))
                    throw new Exception("Operator must be a valid comparison operator! (Ex. >, <, <=, >=, =, !=, is, not)");

                if (value == null)
                    throw new Exception("Must have a valid value!");

                return true;
            }
            catch (Exception ex)
            {
                if (ignoreErrors)
                    return false;

                throw ex;
            }
        }

        private string Convert(Func<int, int, bool> logic)
        {
            // Fill this
            return "";
        }
    }
}