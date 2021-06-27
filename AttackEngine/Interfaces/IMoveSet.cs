using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models.Player;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Interfaces
{
    public interface IMoveSet
        : IContent
    {
        #region Public Properties

        IEnumerable<ComboAction> Actions { get; }
        IEnumerable<ComboRoute> NeutralRoutes { get; }

        #endregion Public Properties

        #region Public Methods

        bool ActionExists(string name);

        ComboAction GetAction(string name);

        ComboRoute GetNeutralRoute(string actionName);

        IEnumerable<ComboRoute> GetRoutesAnywhere(string actionName, ActionInputs input = ActionInputs.NoInput);

        bool NeutralRouteExists(string actionName);

        bool RouteExistsAnywhere(string actionName, ActionInputs input = ActionInputs.NoInput);

        #endregion Public Methods
    }
}