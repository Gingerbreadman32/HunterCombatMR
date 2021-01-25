using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct ComboRoute
    {
        #region Public Constructors

        public ComboRoute(string actionName,
                    int buffer,
                    ComboInputs input,
                    int priority,
                    bool hold = false)
        {
            ComboActionName = actionName;
            BufferFrames = buffer;
            Input = input;
            Priority = priority;
            HoldBuffer = hold;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The name of the <see cref="ComboAction"/> to transition to
        /// </summary>
        public string ComboActionName { get; set; }

        /// <summary>
        /// The amount of frames between when you can input the command and when the attack will come out
        /// </summary>
        public int BufferFrames { get; set; }

        /// <summary>
        /// Whether or not the command can be held down as a means of buffering (Will overwrite priority for buffered inputs not also held)
        /// </summary>
        public bool HoldBuffer { get; set; }

        /// <summary>
        /// The input the combo is waiting for
        /// </summary>
        /// <remarks>
        /// @@warn Have to rework this into something that can take multiple inputs.
        /// </remarks>
        public ComboInputs Input { get; set; }

        /// <summary>
        /// The priority set if multiple commands are buffered. Lowest takes priority and 0 will always take precedent.
        /// </summary>
        /// <remarks>
        /// @@warn Multiple commands in a sequence with a 0 priority should throw an exception.
        /// </remarks>
        public int Priority { get; set; }

        #endregion Public Properties
    }
}