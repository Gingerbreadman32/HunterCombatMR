using HunterCombatMR.Components;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.BehaviorSystem;
using HunterCombatMR.Messages.EntityStateSystem;
using HunterCombatMR.Models.State;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Systems
{
    public class EntityStateSystem
        : ModSystem<EntityStateComponent>,
        IMessageHandler<SetWorldStatusMessage>,
        IMessageHandler<ChangeStateMessage>
    {
        private List<ChangeStateMessage> _changeStateMessages;
        private List<SetWorldStatusMessage> _worldStatusMessages;

        public bool HandleMessage(SetWorldStatusMessage message)
        {
            _worldStatusMessages.Add(message);
            return true;
        }

        public bool HandleMessage(ChangeStateMessage message)
        {
            _changeStateMessages.Add(message);
            return true;
        }

        public override void PostEntityUpdate()
        {
            if (InputCheckingUtils.PlayerInputBufferPaused())
                return;

            ForEachComponentEntity(UpdateEntityStates);

            _worldStatusMessages.Clear();
            _changeStateMessages.Clear();
        }

        protected override void OnCreate()
        {
            _worldStatusMessages = new List<SetWorldStatusMessage>();
            _changeStateMessages = new List<ChangeStateMessage>();
        }

        private static void SetState(ref EntityStateComponent component,
            int stateNumber,
            int entityId)
        {
            if (!ComponentManager.HasComponent<BehaviorComponent>(entityId))
                return;

            var behavior = ComponentManager.GetEntityComponent<BehaviorComponent>(entityId);

            if (!behavior.TryGetState(stateNumber, out var state))
                return;

            component.StateInfo = new StateInfo(stateNumber, state);

            if (state.Definition.Animation.HasValue)
                SystemManager.SendMessage(new SetAnimationRequestMessage(entityId, state.Definition.Animation.Value));
        }

        private void ChangeStateFromMessages(IModEntity entity, ref EntityStateComponent component)
        {
            foreach (var stateMessage in _changeStateMessages.Where(x => x.EntityId.Equals(entity.Id)))
            {
                SetState(ref component, stateMessage.StateNumber, stateMessage.EntityId);
            }
        }

        private void EvaluateControllers(int entityId,
            StateInfo state)
        {
            for (int c = 0; c < state.StateControllerInfo.Length; c++)
            {
                if (EvaluateTriggers(state.StateControllerInfo[c].Definition.Triggers, entityId))
                    StateControllerManager.InvokeController(state.StateControllerInfo[c].Definition.Type, entityId, state.StateControllerInfo[c].Definition.Parameters);
            }
        }

        private bool EvaluateTrigger(StateTrigger trigger,
            int entityId)
        {
            var type = StateTriggerManager.GetParameterComponentType(trigger.Parameter);

            if (!ComponentManager.HasComponent(entityId, type))
                return false;

            ref readonly var component = ref ComponentManager.GetEntityComponent(entityId, type);
            var property = StateTriggerManager.GetComponentProperty(in component, trigger.Parameter);

            return Convert.ToSingle(property) == trigger.Value; // testing
        }

        // Make unit tests for this
        private bool EvaluateTriggers(StateTrigger[][] triggers,
                    int entityId) // Please refactor this, lol
        {
            bool qualified = false;

            for (int s = 0; s < triggers.Length; s++)
            {
                if (triggers[s].Length == 0)
                    continue;

                for (int t = 0; t < triggers[s].Length; t++)
                {
                    var triggerStatus = EvaluateTrigger(triggers[s][t], entityId);

                    if (!triggerStatus)
                    {
                        if (s.Equals(0))
                            return false;

                        qualified = false;
                        break;
                    }

                    qualified = true;
                }

                if (qualified)
                    return qualified;
            }

            return qualified;
        }

        private void SetWorldStatusFromMessages(IModEntity entity, EntityStateComponent component)
        {
            foreach (var worldMessage in _worldStatusMessages.Where(x => x.EntityId.Equals(entity.Id)))
            {
                component.StateInfo.WorldStatus = worldMessage.Status;
            }
        }

        private void UpdateEntityStates(IModEntity entity)
        {
            ref var component = ref entity.GetComponent<EntityStateComponent>();

            SetWorldStatusFromMessages(entity, component);

            EvaluateControllers(entity.Id, component.StateInfo);
            component.StateInfo.Time++;

            ChangeStateFromMessages(entity, ref component);
        }
    }
}