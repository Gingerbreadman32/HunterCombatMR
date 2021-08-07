using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Models.State;
using HunterCombatMR.Utilities;
using System;

namespace HunterCombatMR.Models.Systems
{
    public class EntityStateSystem
        : ModSystem<EntityStateComponent>
    {
        public override void PostEntityUpdate()
        {
            foreach (var entity in ReadEntities())
            {
                ref var component = ref ComponentManager.GetEntityComponent<EntityStateComponent>(entity);

                if (InputCheckingUtils.PlayerInputBufferPaused())
                    continue;

                component.CurrentStateInfo.Time++;

                int newState = EvaluateControllers(entity, component.GetCurrentState());
                if (newState > -1)
                    component.SetState((component.CurrentStateNumber == 0) ? 1 : 0);
            }
        }

        private int EvaluateControllers(IModEntity entity,
            EntityState state)
        {
            for (int c = 0; c < state.Controllers.Length; c++)
            {
                if (EvaluateTriggers(state.Controllers[c].Triggers, entity))
                    return c;
            }

            return -1;
        }

        private bool EvaluateTrigger(StateTrigger trigger,
            IModEntity entity)
        {
            var type = StateTriggerManager.GetParameterComponentType(trigger.Parameter);

            if (!ComponentManager.HasComponent(entity, type))
                return false;

            ref readonly var component = ref ComponentManager.GetEntityComponent(entity, type);
            var property = StateTriggerManager.GetComponentProperty(in component, trigger.Parameter);

            return Convert.ToSingle(property) == trigger.Value; // testing
        }

        // Make unit tests for this
        private bool EvaluateTriggers(StateTrigger[][] triggers,
                    IModEntity entity)
        {
            bool qualified = false;

            for (int s = 0; s < triggers.Length; s++)
            {
                if (triggers[s].Length == 0)
                    continue;

                for (int t = 0; t < triggers[s].Length; t++)
                {
                    var triggerStatus = EvaluateTrigger(triggers[s][t], entity);

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
    }
}