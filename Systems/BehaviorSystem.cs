using HunterCombatMR.Components;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Messages.BehaviorSystem;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.State;
using System.Linq;

namespace HunterCombatMR.Systems
{
    public sealed class BehaviorSystem
        : ModSystem<BehaviorComponent>,
        IMessageHandler<AddBehaviorMessage>,
        IMessageHandler<RemoveBehaviorMessage>,
        IMessageHandler<SetAnimationRequestMessage>
    {
        public bool HandleMessage(AddBehaviorMessage message)
        {
            if (!HasComponent(message.EntityId)) 
            {
                ComponentManager.RegisterComponent(new BehaviorComponent(message.Behavior), message.EntityId);
                return HasComponent(message.EntityId);
            }

            ref var component = ref GetComponent(message.EntityId);

            if (component.Behaviors.Any(x => x.Name.Equals(message.Behavior.Name)))
                return false;

            if (message.Index >= 0)
            {
                component.InsertBehavior(message.Behavior, message.Index);
                return true;
            }

            component.AddBehavior(message.Behavior);
            return true;
        }

        public bool HandleMessage(RemoveBehaviorMessage message)
        {
            return false;
        }

        public bool HandleMessage(SetAnimationRequestMessage message)
        {
            if (!HasComponent(message.EntityId))
                return false;

            var animationResult = GetComponent(message.EntityId).TryGetAnimation(message.AnimationNumber, out CustomAnimation animation);
            CustomAnimation? send = animation;

            if (!animationResult)
                send = null;

            return SystemManager.SendMessage(new ChangeAnimationMessage(message.EntityId, send));
        }

        public override void PreEntityUpdate()
        {
            ForEachComponentEntity(ManageComponents);
        }

        private void ManageComponents(IModEntity entity)
        {
            ref var component = ref GetComponent(in entity);
            if (!entity.HasComponent<EntityStateComponent>() && component.TryGetState(0, out EntityState state))
            {
                entity.RegisterComponent(new EntityStateComponent(new StateInfo(0, state), component.ActiveGlobalControllers.ToArray()));
            }
        }
    }
}