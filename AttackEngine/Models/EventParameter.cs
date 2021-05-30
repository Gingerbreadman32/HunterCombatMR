using HunterCombatMR.Interfaces;

namespace HunterCombatMR.AttackEngine.Models
{
    public class EventParameter
        : IDisplayNamed
    {
        public EventParameter(string name,
            float defaultValue)
        {
            DisplayName = name;
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public EventParameter(EventParameter parameter,
            float newValue)
        {
            DisplayName = parameter.DisplayName;
            DefaultValue = parameter.DefaultValue;
            Value = newValue;
        }

        public float DefaultValue { get; }
        public string DisplayName { get; }

        public float Value { get; set; }
    }
}