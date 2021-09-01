using HunterCombatMR.Components;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Messages.EntityStateSystem;
using HunterCombatMR.Models.Animation;
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
        private List<SetWorldStatusMessage> _worldStatusMessages;
        private List<ChangeStateMessage> _changeStateMessages;

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

            foreach (var entity in ReadEntities())
            {
                ref var component = ref entity.GetComponent<EntityStateComponent>();

                SetWorldStatusFromMessages(entity, component);

                EvaluateControllers(entity.Id, component.GetCurrentState());
                component.CurrentStateInfo.Time++;

                ChangeStateFromMessages(entity, ref component);
            }

            _worldStatusMessages.Clear();
            _changeStateMessages.Clear();
        }

        protected override void OnCreate()
        {
            _worldStatusMessages = new List<SetWorldStatusMessage>();
            _changeStateMessages = new List<ChangeStateMessage>();
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
        }

        private void ChangeStateFromMessages(IModEntity entity, ref EntityStateComponent component)
        {
            foreach (var stateMessage in _changeStateMessages.Where(x => x.EntityId.Equals(entity.Id)))
            {
                SetState(ref component, stateMessage.StateNumber, stateMessage.EntityId);
            }
        }

        private static void SetState(ref EntityStateComponent component,
            int stateNumber,
            int entityId)
        {
            var state = component.GetState(stateNumber);
            component.CurrentStateInfo = new StateInfo(component.CurrentStateInfo,
                component.CurrentStateNumber,
                component.GetState(stateNumber).Definition,
                Array.FindIndex(component.StateSets, x => x.States.ContainsKey(stateNumber)));
            component.CurrentStateNumber = stateNumber;

            if (state.Definition.Animation.HasValue)
            {
                if (!ComponentManager.HasComponent<AnimationComponent>(entityId) && state.Definition.Animation.Value >= 0)
                {
                    ComponentManager.RegisterComponent(new AnimationComponent(component.AnimationSet[state.Definition.Animation.Value]), entityId);
                    return;
                }

                SetAnimation(entityId, state.Definition.Animation.Value, component.AnimationSet);
            }

        }

        private static void SetAnimation(int entityId, 
            int animationIndex,
            CustomAnimation[] animationSet)
        {
            if (animationIndex < 0)
            {
                ComponentManager.RemoveComponent<AnimationComponent>(entityId);
                return;
            }

            SystemManager.SendMessage(new ChangeAnimationMessage(entityId, animationSet[animationIndex]));
        }
    }
}