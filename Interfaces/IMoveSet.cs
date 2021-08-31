using HunterCombatMR.Enumerations;
using HunterCombatMR.Models.MoveSet;
using HunterCombatMR.Models.Player;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces
{
    public interface IMoveSet
        : IContent
    {
        IEnumerable<ComboAction> Actions { get; }
        IEnumerable<ComboRoute> NeutralRoutes { get; }

        bool ActionExists(string name);

        ComboAction GetAction(string name);

        ComboRoute GetNeutralRoute(string actionName);

        IEnumerable<ComboRoute> GetRoutesAnywhere(string actionName, DefinedInputs input = DefinedInputs.NoInput);

        bool NeutralRouteExists(string actionName);

        bool RouteExistsAnywhere(string actionName, DefinedInputs input = DefinedInputs.NoInput);
    }
}