using System;

namespace HunterCombatMR.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TriggerParameter
        : Attribute
    {
        public TriggerParameter(string parameterName)
        {
            Value = parameterName;
        }

        public string Value { get; }
    }
}