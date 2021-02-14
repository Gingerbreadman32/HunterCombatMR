﻿using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerBufferInformation
    {
        #region Private Fields

        private const int _maxSameInput = 6;

        #endregion Private Fields

        #region Public Constructors

        public PlayerBufferInformation()
        {
            BufferedComboInputs = new List<BufferedInput>();
            HeldComboInputs = new Dictionary<ComboInputs, int>();
            PopulateHoldCommands();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// A dictionary containing the list of combo inputs that have been recently pressed and how long since they've been buffered.
        /// </summary>
        public IList<BufferedInput> BufferedComboInputs { get; set; }

        /// <summary>
        /// A dictionary containing a list of the combo inputs that are being held and how long they've been held.
        /// </summary>
        public IDictionary<ComboInputs, int> HeldComboInputs { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void AddToBuffers(ComboInputs input)
        {
            var alreadyBuffered = BufferedComboInputs.Where(x => x.Input.Equals(input));
            if (alreadyBuffered.Count() >= _maxSameInput)
                BufferedComboInputs.Remove(alreadyBuffered.OrderByDescending(x => x.FramesSinceBuffered).First());

            BufferedComboInputs.Add(new BufferedInput(input));
        }

        public void PopulateHoldCommands()
        {
            foreach (ComboInputs input in Enum.GetValues(typeof(ComboInputs)))
            {
                HeldComboInputs.Add(input, 0);
            }
        }

        public void ResetBuffers()
        {
            BufferedComboInputs.Clear();
            ResetHoldTimes();
        }

        public void ResetHoldTimes()
        {
            var tempHeldKeys = new Dictionary<ComboInputs, int>(HeldComboInputs);
            foreach (var held in HeldComboInputs.Keys)
            {
                tempHeldKeys[held] = 0;
            }
            HeldComboInputs = tempHeldKeys;
        }

        public void Update()
        {
            var tempBuffKeys = new List<BufferedInput>(BufferedComboInputs);
            foreach (var binput in BufferedComboInputs)
            {
                if (binput.FramesSinceBuffered < binput.MaximumBufferFrames - 1)
                    binput.AddFramestoBuffer(1);
                else
                    tempBuffKeys.Remove(binput);
            }
            BufferedComboInputs = tempBuffKeys;

            var tempHeldKeys = new Dictionary<ComboInputs, int>(HeldComboInputs);
            foreach (var hinput in HeldComboInputs.Where(x => x.Key.IsPressed()))
            {
                tempHeldKeys[hinput.Key]++;
            }
            HeldComboInputs = tempHeldKeys;

            foreach (ComboInputs input in Enum.GetValues(typeof(ComboInputs)))
            {
                if (input.JustPressed())
                {
                    AddToBuffers(input);
                }
                else if (!input.IsPressed() || input.JustReleased())
                {
                    HeldComboInputs[input] = 0;
                }
            }
        }

        #endregion Public Methods
    }
}