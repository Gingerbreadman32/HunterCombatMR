using System.Collections.Generic;

namespace HunterCombatMR.Models.State
{
    public class StateSet
    {
        public StateSet()
        {
            StateReferences = new List<StateReference>();
        }

        public IEnumerable<StateReference> StateReferences { get; set; }
    }
}