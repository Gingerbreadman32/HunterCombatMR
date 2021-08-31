using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Extensions
{
    public static class EntityExtentions
    {
        public static ref TComponent GetComponent<TComponent>(this IModEntity entity) where TComponent : struct
            => ref ComponentManager.GetEntityComponent<TComponent>(entity.Id);

        public static bool TryGetComponent<TComponent>(this IModEntity entity, out TComponent component) where TComponent : struct
        {
            bool hasComponent = entity.HasComponent<TComponent>();
            component = default(TComponent);

            if (hasComponent)
                component = ComponentManager.GetEntityComponent<TComponent>(entity.Id);

            return hasComponent;
        }

        public static IReadOnlyList<Type> GetComponentTypes(this IModEntity entity)
            => EntityManager.GetEntityComponentTypes(entity.Id);

        public static bool HasComponent<TComponent>(this IModEntity entity) where TComponent : struct
                            => GetComponentTypes(entity).Any(x => x.Equals(typeof(TComponent)));

        public static void RegisterComponent<TComponent>(this IModEntity entity,
            TComponent component) where TComponent : struct
        {
            ComponentManager.RegisterComponent(component, entity.Id);
        }

        public static void RemoveComponent<TComponent>(this IModEntity entity) where TComponent : struct
                    => ComponentManager.RemoveComponent<TComponent>(entity.Id);

        public static void RemoveAllComponents(this IModEntity entity)
        {
            foreach (var component in entity.GetComponentTypes())
            {
                ComponentManager.RemoveComponent(entity.Id, component);
            }
        }

        public static bool SendMessage<TMessage>(this IModEntity entity, TMessage message)
            => SystemManager.SendMessage(message);
    }
}