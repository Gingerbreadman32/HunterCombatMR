namespace HunterCombatMR.Interfaces
{
    public interface IHunterCombatContentInstance
    {
        string InternalName { get; }

        T Duplicate<T>(string name) where T : IHunterCombatContentInstance;
    }
}