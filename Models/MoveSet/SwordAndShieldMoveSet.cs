using HunterCombatMR.Constants;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models.Player;
using System.Collections.Generic;

namespace HunterCombatMR.Models.MoveSet
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

        public override IContent CreateNew(string internalName)
            => new SwordAndShieldMoveSet();

        protected override IEnumerable<ComboAction> PopulateDefaultActions()
        {
            var actions = new List<ComboAction>();

            //actions.Add(new ComboAction(ContentUtils.GetPlayerAction(_doubleSlash)));
            //actions.Add(new ComboAction(ContentUtils.GetPlayerAction(_runningSlash), Enumerations.EntityWorldStatus.Walking));

            return actions;
        }

        protected override IEnumerable<ComboRoute> SetNeutralRoutes()
        {
            var routes = new List<ComboRoute>();
            var neutralstate = new Enumerations.EntityActionStatus[] { Enumerations.EntityActionStatus.Idle };

            routes.Add(new ComboRoute(GetAction("Double Slash"), Enumerations.DefinedInputs.PrimaryAction, neutralstate));
            routes.Add(new ComboRoute(GetAction("Running Slash"), Enumerations.DefinedInputs.PrimaryAction, neutralstate));

            return routes;
        }
    }
}