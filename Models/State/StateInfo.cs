using HunterCombatMR.Enumerations;

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

            IgnoringPhysics = false;
            ActionStatus = EntityActionStatus.Idle;
            WorldStatus = EntityWorldStatus.NoStatus;
            HasControl = true;
        }

        public StateInfo(StateInfo current,
            int prevStateNo,
            StateDef stateDef,
            int setIndex = 0)
        {
            Time = 0;
            StateSet = setIndex;
            PreviousStateNumber = prevStateNo;
            _controllerInfo = new StateControllerInfo[] { };
            PreviousStateSet = current.StateSet;

            IgnoringPhysics = stateDef.IgnorePhysics.GetValueOrDefault(current.IgnoringPhysics);
            ActionStatus = stateDef.ActionStatus;
            WorldStatus = stateDef.WorldStatus;
            HasControl = stateDef.HasControl.GetValueOrDefault(current.HasControl);
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

        /// <summary>
        /// The index of the previous state's state set.
        /// </summary>
        public int PreviousStateSet { get; set; }

        public EntityWorldStatus WorldStatus { get; set; }

        public EntityActionStatus ActionStatus { get; set; }

        public bool IgnoringPhysics { get; set; }

        public bool HasControl { get; set; }
    }
}