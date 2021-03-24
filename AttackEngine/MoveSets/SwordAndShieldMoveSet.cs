using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Utilities;
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
        private const string _doubleSlash = "DoubleSlash";
        private const string _runningSlash = "RunningSlash";

        public SwordAndShieldMoveSet() 
            : base(MoveSetNames.SwordAndShield)
        {
        }

        protected override IEnumerable<ComboAction> PopulateDefaultActions()
        {
            var actions = new List<ComboAction>();

            actions.Add(new ComboAction(ContentUtils.GetPlayerAction(_doubleSlash)));
            actions.Add(new ComboAction(ContentUtils.GetPlayerAction(_runningSlash), Enumerations.PlayerState.Walking));

            return actions;
        }

        protected override IEnumerable<ComboRoute> SetNeutralRoutes()
        {
            var routes = new List<ComboRoute>();
            var neutralstate = new Enumerations.AttackState[] { Enumerations.AttackState.NotAttacking };

            routes.Add(new ComboRoute(GetAction(_doubleSlash), Enumerations.ActionInputs.PrimaryAction, neutralstate));
            routes.Add(new ComboRoute(GetAction(_runningSlash), Enumerations.ActionInputs.PrimaryAction, neutralstate));

            return routes;
        }
    }
}
