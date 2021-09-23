namespace HunterCombatMR.Models.State
{
    public class StateControllerInfo
    {
        public StateControllerInfo(StateController controller,
            int timesFired = 0)
        {
            Definition = controller;
            TimesFired = timesFired;
            Parameters = controller.Parameters;
        }

        public StateController Definition { get; set; }

        public int TimesFired { get; set; }

        /// <summary>
        /// The parameters being passed to the state controller.
        /// </summary>
        public object[] Parameters { get; set; }
    }
}