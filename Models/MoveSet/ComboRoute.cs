﻿using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Models.Player;

namespace HunterCombatMR.Models.MoveSet
{
    public struct ComboRoute
    {
        public ComboRoute(ComboAction action,
                    DefinedInputs input,
                    EntityActionStatus[] states,
                    int buffer = DefaultAttackDetails.DefaultBufferWindow,
                    int priority = DefaultAttackDetails.DefaultInputPriority,
                    int delay = DefaultAttackDetails.DefaultInputDelay,
                    bool hold = false,
                    int holdFrames = 0)
        {
            ComboAction = action;
            InputBufferFrames = buffer;
            Input = input;
            Priority = priority;
            InputDelayFrames = delay;
            InputHold = hold;
            StatesCancellableFrom = states;
            InputHoldFrames = holdFrames;
        }

        /// <summary>
        /// The <see cref="HunterCombatMR.Models.Player.ComboAction"/> to transition to
        /// </summary>
        public ComboAction ComboAction { get; }

        /// <summary>
        /// The input the combo is waiting for
        /// </summary>
        public DefinedInputs Input { get; }

        /// <summary>
        /// The amount of frames before the cancel window of the current action that buffered input will count for.
        /// </summary>
        public int InputBufferFrames { get; }

        /// <summary>
        /// The amount of frames to delay the route until cancelling the current action if pressed after the cancel window has started.
        /// </summary>
        public int InputDelayFrames { get; }

        /// <summary>
        /// Whether or not the command can be held down as a means of buffering (Will overwrite priority for buffered inputs not also held)
        /// </summary>
        public bool InputHold { get; }

        /// <summary>
        /// How long the input must be held to be considered for the action.
        /// </summary>
        /// <remarks>
        /// Only used if InputHold is true.
        /// </remarks>
        public int InputHoldFrames { get; }

        /// <summary>
        /// The priority set if multiple commands are buffered. Lowest takes priority and 0 will always take precedent.
        /// If two routes have the same priority then the one pressed last will break the tie.
        /// </summary>
        /// <remarks>
        /// @@warn Multiple commands in a sequence with a 0 priority should throw an exception.
        /// </remarks>
        public int Priority { get; }

        /// <summary>
        /// All of the attack states the previous action may be in that this action can combo from.
        /// </summary>
        public EntityActionStatus[] StatesCancellableFrom { get; }
    }
}