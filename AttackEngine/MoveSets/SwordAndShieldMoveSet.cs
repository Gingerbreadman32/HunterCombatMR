using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.MoveSets
{
    /// <summary>
    /// Temporary
    /// </summary>
    public class SwordAndShieldMoveSet
        : MoveSet
    {
        private const string _doubleSlash = "DoubleSlash";
        private const string _runningSlash = "RunningSlash";

        public SwordAndShieldMoveSet() 
            : base(MoveSetNames.SwordAndShield)
        {
        }

        public override IHunterCombatContentInstance CloneFrom(string internalName)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ComboAction> PopulateDefaultActions()
        {
            var actions = new List<ComboAction>();

            actions.Add(new ComboAction(ContentUtils.Get<PlayerAction>(_doubleSlash)));
            actions.Add(new ComboAction(ContentUtils.Get<PlayerAction>(_runningSlash), Enumerations.PlayerState.Walking));

            return actions;
        }

        protected override IEnumerable<ComboRoute> SetNeutralRoutes()
        {
            var routes = new List<ComboRoute>();
            var neutralstate = new Enumerations.AttackState[] { Enumerations.AttackState.NotAttacking };

            routes.Add(new ComboRoute(GetAction("Double Slash"), Enumerations.ActionInputs.PrimaryAction, neutralstate));
            routes.Add(new ComboRoute(GetAction("Running Slash"), Enumerations.ActionInputs.PrimaryAction, neutralstate));

            return routes;
        }
    }
}
