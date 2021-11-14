namespace HunterCombatMR.Interfaces.State
{
    public interface IStateControllerType
    {
        string Name { get; }

        int RequiredArguments { get; }

        void Invoke(int entityId, params string[] args);
    }
}