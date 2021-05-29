using HunterCombatMR.Interfaces;

namespace HunterCombatMR
{
    public abstract class HunterCombatContentInstance
        : IHunterCombatContentInstance
    {
        public HunterCombatContentInstance(string internalName)
        {
            InternalName = internalName;
        }

        public string InternalName { get; private set; }

        public bool IsStoredInternally { get; internal set; }

        public abstract IHunterCombatContentInstance CreateNew(string internalName);
    }
}