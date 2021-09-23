using HunterCombatMR.Models.State;

namespace HunterCombatMR.Interfaces.State.Builders
{
    public interface IStateControllerBuilder
    {
        string ControllerType { get; set; }
        bool IgnoreHitPause { get; set; }
        int Persistency { get; set; }

        void AddParameters(params object[] parameters);

        void AddTrigger(StateTrigger trigger, int depth);

        StateController Build();
    }
}