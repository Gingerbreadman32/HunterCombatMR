using HunterCombatMR.Interfaces.Entity;

namespace HunterCombatMR.Interfaces.State
{
    public interface IControllerEffect
    {
        int RequiredArguments { get; }
        string ControllerType { get; }

        void Invoke(in IModEntity entity, params object[] args);
    }
}