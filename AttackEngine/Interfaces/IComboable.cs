using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.AttackEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.Interfaces
{
    public interface IComboable
    {
        /// <summary>
        /// The combo routes the attack is allowed to take
        /// </summary>
        IEnumerable<ComboRoute> Routes { get; }

        /// <summary>
        /// Creates all of the routes that the attack will be able to go down, make sure to call this somewhere (preferrably the constructor)
        /// </summary>
        void EstablishRoutes();

        void CopyRoutes(IEnumerable<ComboRoute> newRoutes);
    }
}
