using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models.Player;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.MoveSet
{
    public abstract class MoveSet
        : Content,
        IMoveSet
    {
        public MoveSet(string name)
            : base(name)
        {
            Actions = PopulateDefaultActions();
            NeutralRoutes = SetNeutralRoutes();
            InitializeMoveSet();
        }

        public IEnumerable<ComboAction> Actions { get; private set; }

        /// <summary>
        /// The actions that can be used from non-action states/neutral.
        /// </summary>
        public IEnumerable<ComboRoute> NeutralRoutes { get; private set; }

        protected virtual IEnumerable<ComboAction> PopulateDefaultActions()
            => new List<ComboAction>();

        protected virtual IEnumerable<ComboRoute> SetNeutralRoutes()
            => new List<ComboRoute>();

        public bool ActionExists(string name)
            => Actions.Any(x => x.DisplayName.Equals(name));

        public ComboAction GetAction(string name)
            => Actions.First(x => x.DisplayName.Equals(name))
                ?? throw new System.Exception($"Requested action {name} does not exist for moveset {InternalName}!");

        public ComboRoute GetNeutralRoute(string actionName)
        {
            if (NeutralRouteExists(actionName))
                return NeutralRoutes.First(x => x.ComboAction.DisplayName.Equals(actionName));
            else
                throw new System.Exception($"Requested neutral route for action {actionName} does not exist for moveset {InternalName}!");
        }

        public IEnumerable<ComboRoute> GetRoutesAnywhere(string actionName,
            DefinedInputs input = DefinedInputs.NoInput)
        {
            if (RouteExistsAnywhere(actionName, input))
                return Actions.Where(x => x.RouteExists(actionName, input)).Select(x => x.GetRoute(actionName, input));
            else
                throw new System.Exception($"Requested neutral route for action {actionName} does not exist for moveset {InternalName}!");
        }

        public bool NeutralRouteExists(string actionName)
                    => NeutralRoutes.Any(x => x.ComboAction.DisplayName.Equals(actionName));

        public bool RouteExistsAnywhere(string actionName,
            DefinedInputs input = DefinedInputs.NoInput)
            => Actions.Any(x => x.RouteExists(actionName, input));

        private void InitializeMoveSet()
        {
            // check and remove duplicate action names (use first)
            // check for routes with the exact same input + player state (use first)
            // Log each
        }
    }
}