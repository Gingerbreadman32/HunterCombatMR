using HunterCombatMR.Models.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.Models.Components
{
    public class PlayerStateComponent
    {
        public PlayerStateComponent()
        {
            StateSets = new SortedList<int, StateSet>();
        }

        public SortedList<int, StateSet> StateSets { get; set; }
    }
}
