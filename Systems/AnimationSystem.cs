using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Systems
{
    public class AnimationSystem
        : ModSystem<AnimationComponent>,
        IMessageHandler<ChangeAnimationMessage>
    {
        private List<Queue<ChangeAnimationMessage>> _changeMessages;

        public IReadOnlyList<Queue<ChangeAnimationMessage>> AnimationChangeQueue { get => _changeMessages; }

        protected override void OnCreate()
        {
            _changeMessages = new List<Queue<ChangeAnimationMessage>>();
        }

        public bool HandleMessage(ChangeAnimationMessage message)
        {
            if (!_changeMessages.Any(x => x.All(y => y.EntityId.Equals(message.EntityId))))
            {
                var queue = new Queue<ChangeAnimationMessage>();
                queue.Enqueue(message);
                _changeMessages.Add(queue);
                return false;
            }

            var current = _changeMessages.Single(x => x.All(y => y.EntityId.Equals(message.EntityId)));
            current.Enqueue(message);
            return true;
        }

        public override void PreEntityUpdate()
        {
            foreach (var entity in ReadEntities())
            {
                ref var component = ref ComponentManager.GetEntityComponent<AnimationComponent>(entity);

                if (InputCheckingUtils.PlayerInputBufferPaused())
                    continue;

                component.Animator.Update();

                var messages = _changeMessages.Where(x => x.All(y => y.EntityId.Equals(entity.Id)));
                foreach (var message in messages)
                {
                    component.Animation = message.Dequeue().Animation;
                }
            }
        }
    }
}