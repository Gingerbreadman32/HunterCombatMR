using HunterCombatMR.Interfaces;

namespace HunterCombatMR
{
    public abstract class HunterCombatContentInstance
        : IHunterCombatContentInstance
    {
        public HunterCombatContentInstance(string name)
        {
            InternalName = name;
        }

        public string InternalName { get; private set; }

        public bool IsStoredInternally { get; internal set; }

        public abstract IHunterCombatContentInstance CloneFrom(string internalName);
    }
}