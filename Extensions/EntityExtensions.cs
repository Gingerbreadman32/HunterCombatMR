using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Managers;

namespace HunterCombatMR.Extensions
{
    public static class EntityExtentions
    {
        public static bool HasComponent<TComponent>(this IModEntity entity) where TComponent : struct
            => ComponentManager.HasComponent<TComponent>(entity);

        public static void RemoveComponent<TComponent>(this IModEntity entity) where TComponent : struct
            => ComponentManager.RemoveComponent<TComponent>(entity);

        public static bool SendMessage<TMessage>(this IModEntity entity, TMessage message)
            => SystemManager.SendMessage(message);

        public static ref TComponent GetComponent<TComponent>(this IModEntity entity) where TComponent : struct
            => ref ComponentManager.GetEntityComponent<TComponent>(entity);

        public static IModEntity WithDefaultComponent<TComponent>(this IModEntity entity) where TComponent : struct
        {
            var component = new TComponent();
            ComponentManager.RegisterComponent(component, entity);

            return entity;
        }
    }
}