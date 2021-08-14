using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.InputSystem;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.Systems
{
    public sealed class InputSystem
        : ModSystem<InputComponent>,
        IMessageHandler<InputResetMessage>
    {
        private IEnumerable<ActionInputs> _concreteInputs;

        public InputSystem()
            : base()
        {
            _concreteInputs = new List<ActionInputs>((ActionInputs[])Enum.GetValues(typeof(ActionInputs))).
                Where(x => !string.IsNullOrEmpty(x.GetGameCommand()));
        }

        public bool HandleMessage(InputResetMessage message)
        {
            if (!EntityManager.EntityExists(message.EntityId))
                return false;

            var entity = EntityManager.GetEntity(message.EntityId);

            if (!entity.HasComponent<InputComponent>())
                return false;

            ResetBuffers(entity.GetComponent<InputComponent>().BufferedInputs);

            return true;
        }

        public override void PostInputUpdate()
        {
            foreach (var entity in ReadEntities())
            {
                ref var component = ref entity.GetComponent<InputComponent>();

                if (InputCheckingUtils.PlayerInputBufferPaused())
                {
                    if (InputCheckingUtils.NoGameInputExists()
                            && !component.BufferedInputs.Any())
                        ResetBuffers(component.BufferedInputs);

                    return;
                }

                component.BufferedInputs = UpdateInputBuffers(component.BufferedInputs);

                if (!(Main.blockInput && !component.IgnoreBlockInput))
                    CheckInputs(component);
            }
        }

        private void CheckInputs(InputComponent component)
        {
            foreach (ActionInputs input in _concreteInputs)
            {
                SetNewestInput(input, component);
            }
        }

        private void ResetBuffers(Queue<BufferedInput> bufferedInputs)
        {
            bufferedInputs.Clear();
        }

        private void SetNewestInput(ActionInputs input,
            InputComponent component)
        {
            bool mouseInterface = component.Player >= 0 ? Main.player[component.Player].mouseInterface : false;

            if (input.JustPressed() && (!input.IsMouse() || !mouseInterface))
            {
                component.BufferedInputs.Enqueue(new BufferedInput(input));
            }
        }

        private Queue<BufferedInput> UpdateInputBuffers(Queue<BufferedInput> componentInputs)
        {
            var inputArray = componentInputs.ToList();
            var markedDelete = new List<int>();

            for (int i = 0; i < inputArray.Count; i++)
            {
                inputArray[i].Update();
                if (inputArray[i].MarkedForDeletion)
                    markedDelete.Add(i);
            }

            int deleted = 0;

            for (int i = 0; i < markedDelete.Count; i++)
            {
                inputArray.RemoveAt(markedDelete[i] - deleted);
                deleted++;
            }

            return new Queue<BufferedInput>(inputArray);
        }
    }
}