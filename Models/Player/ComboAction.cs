using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models.MoveSet;
using HunterCombatMR.Models.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Player
{
    public class ComboAction
        : IDisplayNamed
    {
        public ComboAction(string name,
            EntityState attack,
            IEnumerable<ComboRoute> routes,
            EntityWorldStatus state = EntityWorldStatus.NoStatus)
        {
            Attack = attack;
            Routes = routes ?? throw new ArgumentNullException(nameof(routes));
            DisplayName = name;
            PlayerStateRequired = state;
        }

        public ComboAction(string name,
            EntityState attack,
            EntityWorldStatus state = EntityWorldStatus.NoStatus)
        {
            Attack = attack;
            DisplayName = name;
            Routes = new List<ComboRoute>();
            PlayerStateRequired = state;
        }

        public EntityState Attack { get; }

        public string DisplayName { get; }

        /// <summary>
        /// What state the player needs to be in in order to perform the action.
        /// </summary>
        public EntityWorldStatus PlayerStateRequired { get; set; }

        public IEnumerable<ComboRoute> Routes { get; set; }

        public void AddRoute(ComboAction action,
            DefinedInputs input,
            EntityActionStatus[] states)
        {
            AddRouteInternal(new ComboRoute(action, input, states));
        }

        public void AddRoute(ComboRoute route)
        {
            AddRouteInternal(route);
        }

        public ComboRoute GetRoute(string actionName,
            DefinedInputs input)
        {
            if (RouteExists(actionName, input))
                return Routes.Single(y => y.ComboAction.DisplayName.Equals(actionName)
                    && !input.Equals(DefinedInputs.NoInput) ? y.Input.Equals(input) : true);
            else
                throw new Exception($"Combo route for action {actionName} with input {input.ToString()} does not exist in relation to current action: {DisplayName}");
        }

        public bool RouteExists(string actionName,
                    DefinedInputs input)
            => Routes.Any(y => y.ComboAction.DisplayName.Equals(actionName)
                && !input.Equals(DefinedInputs.NoInput) ? y.Input.Equals(input) : true);

        private void AddRouteInternal(ComboRoute route)
        {
            var routes = new List<ComboRoute>(Routes);

            routes.Add(route);

            Routes = routes;
        }
    }
}