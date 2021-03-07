using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct ComboRoute
    {
        #region Public Constructors

        public ComboRoute(ComboAction action,
                    ActionInputs input,
                    int buffer = DefaultAttackDetails.DefaultBufferWindow,
                    int priority = DefaultAttackDetails.DefaultInputPriority,
                    bool hold = false)
        {
            ComboAction = action;
            BufferFrames = buffer;
            Input = input;
            Priority = priority;
            HoldBuffer = hold;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The <see cref="Models.ComboAction"/> to transition to
        /// </summary>
        public ComboAction ComboAction { get; }

        /// <summary>
        /// The amount of frames before the recovery of the current action that buffered input will register
        /// </summary>
        public int BufferFrames { get; }

        /// <summary>
        /// Whether or not the command can be held down as a means of buffering (Will overwrite priority for buffered inputs not also held)
        /// </summary>
        public bool HoldBuffer { get; }

        /// <summary>
        /// The input the combo is waiting for
        /// </summary>
        public ActionInputs Input { get; }

        /// <summary>
        /// The priority set if multiple commands are buffered. Lowest takes priority and 0 will always take precedent.
        /// If two routes have the same priority then the one pressed last will break the tie.
        /// </summary>
        /// <remarks>
        /// @@warn Multiple commands in a sequence with a 0 priority should throw an exception.
        /// </remarks>
        public int Priority { get; }

        #endregion Public Properties
    }
}