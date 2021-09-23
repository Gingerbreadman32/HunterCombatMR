using HunterCombatMR.Enumerations;
using System.Linq;

namespace HunterCombatMR.Models.State
{
    /// <summary>
    /// Stores all of the information about the current state of the entity; Exists only within the lifetime of the current state
    /// </summary>
    public class StateInfo
    {
        private StateControllerInfo[] _controllerInfo;
        private StateDef _definition;
        private int _previousStateNumber;
        private int _stateNumber;

        public StateInfo(int stateNo,
            EntityState state)
        {
            _stateNumber = -1;
            HasControl = true;

            SetToState(stateNo, state);
        }

        /// <summary>
        /// The entity's current action status within the state
        /// </summary>
        public EntityActionStatus ActionStatus { get; set; }

        /// <summary>
        /// The definition of the state, used to reset the state to its default parameters
        /// </summary>
        public StateDef Definition
        {
            get => _definition;
            set => _definition = value;
        }

        public bool HasControl { get; set; }

        /// <summary>
        /// The state number of the previous state
        /// </summary>
        public int PreviousStateNumber { get => _previousStateNumber; }

        public StateControllerInfo[] StateControllerInfo { get => _controllerInfo; }
        public int StateNumber { get => _stateNumber; }

        /// <summary>
        /// How long the entity has been in the current state
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// The entity's current world status within the state
        /// </summary>
        public EntityWorldStatus WorldStatus { get; set; }

        public void SetDefinition(StateDef definition)
        {
            _definition = definition;
            WorldStatus = definition.WorldStatus;
            ActionStatus = definition.ActionStatus;
            HasControl = definition.HasControl.HasValue ? definition.HasControl.Value : HasControl;
        }

        public void SetToState(int stateNo,
            EntityState state,
            bool resetTimer = true)
        {
            if (resetTimer)
                Time = 0;

            _previousStateNumber = StateNumber;
            _stateNumber = stateNo;

            _controllerInfo = state.Controllers.Select(x => new StateControllerInfo(x)).ToArray();

            SetDefinition(state.Definition);
        }
    }
}