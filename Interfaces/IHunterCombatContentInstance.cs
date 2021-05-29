namespace HunterCombatMR.Interfaces
{
    public interface IHunterCombatContentInstance
    {
        string InternalName { get; }

        bool IsStoredInternally { get; }
        IHunterCombatContentInstance CreateNew(string internalName);
    }
}