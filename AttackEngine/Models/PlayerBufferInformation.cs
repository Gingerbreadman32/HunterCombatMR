using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerBufferInformation
    {
        private const int _maxSameInput = 3;
        private IEnumerable<ActionInputs> _concreteInputs;

        public PlayerBufferInformation()
        {
            BufferedComboInputs = new List<BufferedInput>();
            HeldComboInputs = new List<HeldInput>();
        }

        /// <summary>
        /// A dictionary containing the list of combo inputs that have been recently pressed and how
        /// long since they've been buffered.
        /// </summary>
        public List<BufferedInput> BufferedComboInputs { get; set; }

        /// <summary>
        /// A dictionary containing a list of the combo inputs that are being held and how long
        /// they've been held.
        /// </summary>
        public List<HeldInput> HeldComboInputs { get; set; }

        public void AddToBuffers(ActionInputs input)
        {
            var alreadyBuffered = BufferedComboInputs.Where(x => x.Input.Equals(input));

            if (alreadyBuffered.Count() >= _maxSameInput)
                BufferedComboInputs.Remove(alreadyBuffered.OrderByDescending(x => x.FramesSinceBuffered).First());

            if (!HunterCombatMR.Instance.VanillaBlockInput && !TerrariaMainUtils.GameInputNotAccepted())
                BufferedComboInputs.Add(new BufferedInput(input));
        }

        public void Initialize()
        {
            _concreteInputs = new List<ActionInputs>((ActionInputs[])Enum.GetValues(typeof(ActionInputs))).
                Where(x => !string.IsNullOrEmpty(x.GetGameCommand()));
            PopulateHoldCommands();
        }

        public void PopulateHoldCommands()
        {
            foreach (ActionInputs input in _concreteInputs)
            {
                HeldComboInputs.Add(new HeldInput(input));
            }
        }

        public void ResetBuffers()
        {
            if (BufferedComboInputs.Any())
                BufferedComboInputs.Clear();

            if (HeldComboInputs.Any(x => x.FramesHeld != 0))
                ResetHoldTimes();
        }

        public void ResetHoldTimes()
        {
            HeldComboInputs = HeldComboInputs.Select(x => new HeldInput(x.Input, 0)).ToList();
        }

        public void Update(PlayerState state,
            bool inInterface)
        {
            BufferedComboInputs = BufferedComboInputs
                .Select(x => { x.AddFramestoBuffer(1); return x; })
                .ToList();
            BufferedComboInputs.RemoveAll(x => x.FramesSinceBuffered >= x.MaximumBufferFrames);

            HeldComboInputs = HeldComboInputs
                .Select(x => { if (x.Input.IsPressed()) { x.FramesHeld++; }; return x; })
                .ToList();

            CheckInputs(state, inInterface);
        }

        private void CheckInputs(PlayerState state,
            bool inInterface)
        {
            foreach (ActionInputs input in _concreteInputs)
            {
                if ((input.JustPressed() && !state.Equals(PlayerState.Dead))
                        && (!input.IsMouse() || !inInterface))
                {
                    AddToBuffers(input);
                }

                if ((!input.IsPressed() || input.JustReleased()) && HeldComboInputs.First(x => x.Input.Equals(input)).FramesHeld != 0)
                {
                    HeldComboInputs.First(x => x.Input.Equals(input)).FramesHeld = 0;
                }
            }
        }
    }
}