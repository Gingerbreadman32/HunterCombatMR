namespace HunterCombatMR.Interfaces
{
    public interface IContent
    {
        string InternalName { get; }

        bool IsStoredInternally { get; }
        IContent CreateNew(string internalName);
    }
}