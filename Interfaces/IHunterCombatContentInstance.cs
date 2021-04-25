namespace HunterCombatMR.Interfaces
{
    public interface IHunterCombatContentInstance
    {
        string InternalName { get; }

        IHunterCombatContentInstance CloneFrom(string internalName);
    }
}