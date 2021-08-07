namespace HunterCombatMR.Models.State
{
    /// <summary>
    /// Stores all of the information about the current state of the entity. Exists only within the lifetime of the current state.
    /// </summary>
    public class StateInfo
    {
        private StateControllerInfo[] _controllerInfo;

        public StateControllerInfo[] StateControllerInfo { get => _controllerInfo; }

        public StateInfo()
        {
            Time = 0;
            StateSet = 0;
            PreviousStateNumber = -1;
            _controllerInfo = new StateControllerInfo[] { };
        }

        public StateInfo(int setIndex,
            int prevStateNo)
        {
            Time = 0;
            StateSet = setIndex;
            PreviousStateNumber = prevStateNo;
            _controllerInfo = new StateControllerInfo[] { };
        }

        /// <summary>
        /// How long the entity has been in the current state.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// The stateset this state is associated with.
        /// </summary>
        public int StateSet { get; set; }

        /// <summary>
        /// The state number of the previous state.
        /// </summary>
        public int PreviousStateNumber { get; set; }
    }
}