using System.Collections.Generic;

namespace HunterCombatMR.Interfaces
{
    public interface IDebuggable
    {
        IDictionary<string, string> DebugData { get; set; }

        string ListData();
    }
}