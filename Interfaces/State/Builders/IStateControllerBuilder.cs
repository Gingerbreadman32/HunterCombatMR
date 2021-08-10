using HunterCombatMR.Constants;
using HunterCombatMR.Models.State;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces.State.Builders
{
    public interface IStateControllerBuilder
    {
        string ControllerType { get; set; }
        bool IgnoreHitPause { get; set; }
        int Persistency { get; set; }

        void AddParameters(IEnumerable<object> parameters);
        void AddTrigger(StateTrigger trigger, int depth);
        StateController Build();
    }
}