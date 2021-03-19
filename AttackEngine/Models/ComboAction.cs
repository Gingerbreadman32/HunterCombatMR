using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AttackEngine.Models
{
    public class ComboAction
    {
        #region Public Constructors

        public ComboAction(PlayerAction attack,
            IEnumerable<ComboRoute> routes,
            PlayerState state = PlayerState.Neutral,
            string name = null)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            Routes = routes ?? throw new ArgumentNullException(nameof(routes));
            Name = name ?? Attack.Name;
            PlayerStateRequired = state;
        }

        public ComboAction(PlayerAction attack,
            PlayerState state = PlayerState.Neutral)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            Name = Attack.Name;
            Routes = new List<ComboRoute>();
            PlayerStateRequired = state;
        }

        #endregion Public Constructors

        #region Public Properties

        public PlayerAction Attack { get; }

        public string Name { get; }

        public IEnumerable<ComboRoute> Routes { get; set; }

        /// <summary>
        /// What state the player needs to be in in order to perform the action.
        /// </summary>
        public PlayerState PlayerStateRequired { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void AddRoute(ComboAction action,
            ActionInputs input,
            AttackState[] states)
        {
            AddRouteInternal(new ComboRoute(action, input, states));
        }

        public void AddRoute(ComboRoute route)
        {
            AddRouteInternal(route);
        }

        public bool RouteExists(string actionName,
            ActionInputs input)
            => Routes.Any(y => y.ComboAction.Name.Equals(actionName)
                && (!input.Equals(ActionInputs.NoInput)) ? y.Input.Equals(input) : true);

        public ComboRoute GetRoute(string actionName,
            ActionInputs input)
        {
            if (RouteExists(actionName, input))
                return Routes.Single(y => y.ComboAction.Name.Equals(actionName)
                    && (!input.Equals(ActionInputs.NoInput)) ? y.Input.Equals(input) : true);
            else
                throw new Exception($"Combo route for action {actionName} with input {input.ToString()} does not exist in relation to current action: {Name}");
        }
        #endregion Public Methods

        #region Private Methods

        private void AddRouteInternal(ComboRoute route)
        {
            var routes = new List<ComboRoute>(Routes);

            routes.Add(route);

            Routes = routes;
        }

        #endregion Private Methods
    }
}