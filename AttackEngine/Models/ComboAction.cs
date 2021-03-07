using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public class ComboAction
    {
        #region Public Constructors

        public ComboAction(Attack attack,
            IEnumerable<ComboRoute> routes,
            string name = null)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            Routes = routes ?? throw new ArgumentNullException(nameof(routes));
            Name = name ?? Attack.Name;
        }

        public ComboAction(Attack attack)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            Name = Attack.Name;
            Routes = new List<ComboRoute>();
        }

        #endregion Public Constructors

        #region Public Properties

        public Attack Attack { get; }

        public string Name { get; }
        public IEnumerable<ComboRoute> Routes { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void AddRoute(ComboAction action,
            ActionInputs input)
        {
            AddRouteInternal(new ComboRoute(action, input));
        }

        public void AddRoute(ComboRoute route)
        {
            AddRouteInternal(route);
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