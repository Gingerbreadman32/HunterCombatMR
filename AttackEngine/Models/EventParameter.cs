using HunterCombatMR.Interfaces;

namespace HunterCombatMR.AttackEngine.Models
{
    public class EventParameter
        : INamed
    {
        public EventParameter(string name,
            float defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public EventParameter(EventParameter parameter,
            float newValue)
        {
            Name = parameter.Name;
            DefaultValue = parameter.DefaultValue;
            Value = newValue;
        }

        public float DefaultValue { get; }
        public string Name { get; }

        public float Value { get; set; }
    }
}