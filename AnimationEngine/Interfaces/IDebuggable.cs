using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IDebuggable
    {
        IDictionary<string, string> DebugData { get; set; }

        string ListData();
    }
}