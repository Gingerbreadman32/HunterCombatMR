using HunterCombatMR.Enumerations;
using HunterCombatMR.Models.State;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces.State.Builders
{
    public interface IStateBuilder
    {
        EntityActionStatus ActionStatus { get; set; }
        bool? HasControl { get; set; }
        bool? IgnorePhysics { get; set; }
        int? SetAnimation { get; set; }
        Vector2? SetVelocity { get; set; }
        EntityWorldStatus WorldStatus { get; set; }

        void AddController(StateController controller);

        void AddControllers(IEnumerable<StateController> controller);

        EntityState Build();
    }
}