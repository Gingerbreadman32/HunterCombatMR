using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Managers;

namespace HunterCombatMR.Extensions
{
    public static class EntityExtentions
    {
        public static IModEntity WithDefaultComponent<TComponent>(this IModEntity entity) where TComponent : struct
        {
            var component = new TComponent();
            ComponentManager.RegisterComponent(component, entity);

            return entity;
        }
    }
}