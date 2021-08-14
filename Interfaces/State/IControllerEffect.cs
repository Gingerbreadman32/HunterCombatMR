namespace HunterCombatMR.Interfaces.State
{
    public interface IControllerEffect
    {
        string ControllerType { get; }
        int RequiredArguments { get; }

        void Invoke(int entityId, params object[] args);
    }
}