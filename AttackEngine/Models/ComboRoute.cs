using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Terraria.GameInput;

namespace HunterCombatMR.AttackEngine.Models
{
    public class ComboRoute
    {
        /// <summary>
        /// The name of the attack to transition to
        /// </summary>
        public string AttackNameReference { get; set; }

        /// <summary>
        /// The keyframe that the attack will cancel the current one out of
        /// </summary>
        public int KeyFrameExecuted { get; set; }

        /// <summary>
        /// The amount of frames between when you can input the command and when the attack will come out
        /// </summary>
        public int BufferFrames { get; set; }

        /// <summary>
        /// The input the combo is waiting for
        /// </summary>
        public ComboInputs Input { get; set; }

        /// <summary>
        /// The priority set if multiple commands are buffered. Lowest takes priority and 0 will always take precedent.
        /// </summary>
        /// <remarks>
        /// Multiple commands in a sequence with a 0 priority should throw an exception.
        /// </remarks>
        public int Priority { get; set; }

        /// <summary>
        /// Whether or not the command can be held down as a means of buffering (Will overwrite priority for buffered inputs not also held)
        /// </summary>
        public bool HoldBuffer { get; set; }

        public ComboRoute(string attackName,
            int keyFrameExecuted,
            int buffer,
            ComboInputs input,
            int priority,
            bool hold = false)
        {
            AttackNameReference = attackName;
            KeyFrameExecuted = keyFrameExecuted;
            BufferFrames = buffer;
            Input = input;
            Priority = priority;
            HoldBuffer = hold;
        }
    }
}