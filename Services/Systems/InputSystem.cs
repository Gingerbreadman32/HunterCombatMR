using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Models.Messages.InputSystem;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.Services.Systems
{
    public sealed class InputSystem
        : ModSystem,
        IMessageHandler<InputResetMessage>,
        IMessageHandler<InputMouseInterfaceMessage>
    {
        private const int _maxBufferedInputs = 60;
        private IEnumerable<ActionInputs> _concreteInputs;
        private bool _mouseInterface;

        public InputSystem()
            : base()
        {
            BufferedInputs = new BufferedInput[_maxBufferedInputs];
            Initialize();
        }

        /// <summary>
        /// An containing the list of combo inputs that have been recently pressed and how
        /// long since they've been buffered.
        /// </summary>
        public BufferedInput[] BufferedInputs { get; set; }

        public bool HandleMessage(InputResetMessage message)
        {
            bool mainPlayer = message.Player == Main.myPlayer;
            if (mainPlayer)
                ResetBuffers();
            return mainPlayer;
        }

        public bool HandleMessage(InputMouseInterfaceMessage message)
        {
            bool wasOn = _mouseInterface == message.MouseInterface;
            _mouseInterface = message.MouseInterface;
            return wasOn;
        }

        public void ResetBuffers()
        {
            for (int i = 0; i < _maxBufferedInputs; i++)
            {
                BufferedInputs[i].Reset();
            }
        }

        public void SetNewestInput(ActionInputs input)
        {
            for (int i = 0; i < _maxBufferedInputs; i++)
            {
                if (BufferedInputs[i].Input.Equals(ActionInputs.NoInput)
                        && BufferedInputs[i].FramesSinceBuffered > 0)
                {
                    BufferedInputs[i].Reset();
                    BufferedInputs[i].Input = input;
                }
            }
        }

        public override void PostInputUpdate()
        {
            if (TerrariaMainUtils.GameInputNotAccepted())
            {
                if (TerrariaMainUtils.NoGameInputAllowed()
                        && BufferedInputs.Any())
                    ResetBuffers();
                return;
            }

            CheckInputs();
            for (int i = 0; i < _maxBufferedInputs; i++)
            {
                if (!(BufferedInputs[i].Input.Equals(ActionInputs.NoInput)
                        && BufferedInputs[i - 1].Input.Equals(ActionInputs.NoInput)))
                    BufferedInputs[i].Update();
            }
        }

        private void CheckInputs()
        {
            foreach (ActionInputs input in _concreteInputs)
            {
                if (input.JustPressed() && (!input.IsMouse() || !_mouseInterface))
                {
                    SetNewestInput(input);
                }
            }
        }

        private void Initialize()
        {
            _concreteInputs = new List<ActionInputs>((ActionInputs[])Enum.GetValues(typeof(ActionInputs))).
                Where(x => !string.IsNullOrEmpty(x.GetGameCommand()));
            for (int i = 0; i < _maxBufferedInputs; i++)
            {
                BufferedInputs[i] = new BufferedInput();
            }
        }
    }
}