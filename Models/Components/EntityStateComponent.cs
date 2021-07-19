using HunterCombatMR.Models.State;
using System.Collections.Generic;

namespace HunterCombatMR.Models.Components
{
    public class EntityStateComponent
        : ModComponent
    {
        public EntityStateComponent()
            : base()
        {
            StateSets = new SortedList<int, StateSet>();
        }

        public SortedList<int, StateSet> StateSets { get; set; }

        public int CurrentState { get; set; }
    }
}