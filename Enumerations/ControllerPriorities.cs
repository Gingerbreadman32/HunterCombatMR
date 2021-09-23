namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// Common priority numbers for <see cref="GlobalStateController"/>; 
    /// Any priority above 1 will not be read in foreign behavior states.
    /// </summary>
    public enum ControllerPriorities
    {
        /// <summary>
        /// Use this for any global controller that involves reading input; Will always be read
        /// </summary>
        Command = 0,

        /// <summary>
        /// Common priority used for most global controllers; Will always be read
        /// </summary>
        Global = 1,

        /// <summary>
        /// Priority used for global controllers that should only be run while running native behaviors;
        /// Will not be read if in a state from another entity's behavior
        /// </summary>
        Local = 2
    }
}