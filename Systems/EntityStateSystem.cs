using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.EntityStateSystem;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Models.State;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Systems
{
    public class EntityStateSystem
        : ModSystem<EntityStateComponent>,
        IMessageHandler<SetWorldStatusMessage>
    {
        private List<SetWorldStatusMessage> _worldStatusMessages;

        public bool HandleMessage(SetWorldStatusMessage message)
        {
            _worldStatusMessages.Add(message);
            return true;
        }

        public override void PostEntityUpdate()
        {
            foreach (var entity in ReadEntities())
            {
                ref var component = ref entity.GetComponent<EntityStateComponent>();

                SetWorldStatusFromMessages(entity, component);

                if (InputCheckingUtils.PlayerInputBufferPaused())
                    continue;

                component.CurrentStateInfo.Time++;
                EvaluateControllers(entity.Id, component.GetCurrentState());
            }
        }

        protected override void OnCreate()
        {
            _worldStatusMessages = new List<SetWorldStatusMessage>();
        }

        private void EvaluateControllers(int entityId,
            EntityState state)
        {
            for (int c = 0; c < state.Controllers.Length; c++)
            {
                if (EvaluateTriggers(state.Controllers[c].Triggers, entityId))
                    StateControllerManager.InvokeController(state.Controllers[c].Type, entityId, state.Controllers[c].Parameters);
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
                    int entityId)
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
                component.CurrentStateInfo.WorldStatus = worldMessage.Status;
            }

            _worldStatusMessages.RemoveAll(x => x.EntityId.Equals(entity.Id));
        }
    }
}