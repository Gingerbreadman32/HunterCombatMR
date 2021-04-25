using HunterCombatMR.Interfaces;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class EventParameter
        : INamed
    {
        public float DefaultValue { get; }
        public string Name { get; }

        public float Value { get; set; }
    }
}