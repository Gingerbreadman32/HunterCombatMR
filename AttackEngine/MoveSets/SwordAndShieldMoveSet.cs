using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.AttackEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.MoveSets
{
    public class SwordAndShieldMoveSet
        : MoveSet
    {
        public SwordAndShieldMoveSet() 
            : base(MoveSetNames.SwordAndShield)
        {
        }

        protected override IEnumerable<ComboAction> PopulateActions()
        {
            return base.PopulateActions();
        }

        protected override IEnumerable<ComboRoute> SetStartingRoutes()
        {
            return base.SetStartingRoutes();
        }
    }
}
