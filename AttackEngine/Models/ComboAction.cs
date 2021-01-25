using System;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct ComboAction
    {
        #region Public Constructors

        public ComboAction(string name,
            Attack attack,
            IEnumerable<ComboRoute> routes)
        {
            Attack = attack ?? throw new ArgumentNullException(nameof(attack));
            Routes = routes ?? new List<ComboRoute>();
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        #endregion Public Constructors

        #region Public Properties

        public Attack Attack { get; set; }

        public string Name { get; set; }
        public IEnumerable<ComboRoute> Routes { get; set; }

        #endregion Public Properties
    }
}