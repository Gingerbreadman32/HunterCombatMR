using HunterCombatMR.Attributes;
using HunterCombatMR.Components;

namespace HunterCombatMR.Constants
{
    /// <summary>
    /// Parameters that associate a component with a trigger script value
    /// Components required are specified and any entity without said component will not evaluate a trigger with that parameter.
    /// </summary>
    public static class ComponentTriggerParams
    {
        /// <summary>
        /// The current frame time of the entity.
        /// </summary>
        [ComponentDependency(typeof(AnimationComponent))]
        public const string FrameTime = "frame";

        /// <summary>
        /// The current key frame time of the entity.
        /// </summary>
        [ComponentDependency(typeof(AnimationComponent))]
        public const string KeyframeTime = "kframe";

        /// <summary>
        /// [DrawComponent] The current x position of the entity relative to the screen.
        /// </summary>
        public const string ScreenPositionX = "posx";

        /// <summary>
        /// [DrawComponent] The current y position of the entity relative to the screen.
        /// </summary>
        public const string ScreenPositionY = "posy";

        /// <summary>
        /// The time the entity has been in its current state.
        /// </summary>
        [ComponentDependency(typeof(EntityStateComponent))]
        public const string StateTime = "time";

        /// <summary>
        /// The state number of the current state of the entity.
        /// </summary>
        [ComponentDependency(typeof(EntityStateComponent))]
        public const string StateNumber = "state";

        /// <summary>
        /// The full array of state variables for the current entity
        /// </summary>
        [ComponentDependency(typeof(EntityStateComponent))]
        public const string StateVariables = "statevars";

        /// <summary>
        /// [VanillaEntityComponent] The current x-based velocity of the entity.
        /// </summary>
        public const string VelocityX = "velx";

        /// <summary>
        /// [VanillaEntityComponent] The current y-based velocity of the entity.
        /// </summary>
        public const string VelocityY = "vely";

        /// <summary>
        /// [VanillaEntityComponent] The current x position of the entity in the world.
        /// </summary>
        public const string WorldPositionX = "worldposx";

        /// <summary>
        /// [VanillaEntityComponent] The current y position of the entity in the world.
        /// </summary>
        public const string WorldPositionY = "worldposy";
    }
}