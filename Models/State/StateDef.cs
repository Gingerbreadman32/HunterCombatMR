using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;

namespace HunterCombatMR.Models.State
{
    /// <summary>
    /// State definition parameters. Defines how the state will affect the entity when first entering it.
    /// </summary>
    public struct StateDef
    {
        public StateDef(EntityWorldStatus worldStatus,
            EntityActionStatus actionStatus,
            bool? physicsOff = null,
            int? animation = null,
            Vector2? velocity = null,
            bool? control = null)
        {
            WorldStatus = worldStatus;
            ActionStatus = actionStatus;
            IgnorePhysics = (physicsOff == null) ? new bool?() : physicsOff;
            Animation = (animation == null) ? new int?() : animation;
            SetVelocity = (velocity == null) ? new Vector2?() : velocity;
            HasControl = (control == null) ? new bool?() : control;
        }

        /// <summary>
        /// Sets the entity's action status. Helps determine when to be able to make certain types of chaining actions.
        /// </summary>
        public EntityActionStatus ActionStatus { get; }

        /// <summary>
        /// Optional parameter which sets the entity's animation if it has an animation component.
        /// </summary>
        public int? Animation { get; }

        /// <summary>
        /// Optional parameter to override entities with an input component, either preventing or allowing input to affect state logic.
        /// </summary>
        public bool? HasControl { get; }

        /// <summary>
        /// Whether or not to ignore the physics system. Will dissalow natural velocity and world status changes until specified otherwise.
        /// </summary>
        public bool? IgnorePhysics { get; }

        /// <summary>
        /// Optional parameter which sets the entity's velocity to this value for the duration of the state. This will be affected by physics.
        /// </summary>
        public Vector2? SetVelocity { get; }

        /// <summary>
        /// Sets the entity's world status upon entering state. This affects physics and can be used to check against for triggers.
        /// </summary>
        public EntityWorldStatus WorldStatus { get; }
    }
}