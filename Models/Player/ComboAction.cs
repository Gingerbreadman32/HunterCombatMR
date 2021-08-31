using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Action;
using HunterCombatMR.Models.MoveSet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Player
{
    public class ComboAction
        : IDisplayNamed
    {
        public ComboAction(ICustomAction<HunterCombatPlayer> attack,
            IEnumerable<ComboRoute> routes,
            EntityWorldStatus state = EntityWorldStatus.NoStatus,
            string name = null)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            Routes = routes ?? throw new ArgumentNullException(nameof(routes));
            DisplayName = name ?? Attack.DisplayName;
            PlayerStateRequired = state;
        }

        public ComboAction(ICustomAction<HunterCombatPlayer> attack,
            EntityWorldStatus state = EntityWorldStatus.NoStatus)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            DisplayName = Attack.DisplayName;
            Routes = new List<ComboRoute>();
            PlayerStateRequired = state;
        }

        public ICustomAction<HunterCombatPlayer> Attack { get; }

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