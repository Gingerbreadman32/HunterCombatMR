using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Services;
using HunterCombatMR.Utilities;
using System;
using System.Linq;

namespace HunterCombatMR.Managers
{
    public sealed class ComponentManager
        : ManagerBase
    {
        public static ref TComponent GetEntityComponent<TComponent>(in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);

            return ref GetEntityComponent<TComponent>(entity.Id);
        }

        public static ref TComponent GetEntityComponent<TComponent>(int entityId) where TComponent : struct
        {
            EntityCheck(entityId);

            int idIndex = GetEntityComponentIndex<TComponent>(entityId);

            return ref ComponentData<TComponent>.EntityComponents[idIndex];
        }

        public static ref TComponent GetGlobalComponent<TComponent>() where TComponent : struct
        {
            return ref ComponentData<TComponent>.GlobalComponent;
        }

        public static bool HasComponent<TComponent>(in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);

            return HasComponent<TComponent>(entity.Id);
        }

        public static bool HasComponent<TComponent>(int entityId) where TComponent : struct
        {
            EntityCheck(entityId);
            var idReference = GetEntityComponentIndex<TComponent>(entityId);
            return idReference > -1;
        }

        public static void RegisterComponent<TComponent>(TComponent component, in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);

            AddOrReplaceEntityComponent(component, entity.Id);
        }

        public static void RemoveComponent<TComponent>(in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);

            if (!HasComponent<TComponent>(entity))
                throw new Exception("Entity has no components of this type!");

            int id = entity.Id;
            RemoveComponent<TComponent>(id);
        }

        protected override void OnDispose()
        {
            ComponentDataManager.Dispose?.Invoke();
        }

        protected override void OnInitialize()
        {
            if (ComponentDataManager.Dispose == null)
                ComponentDataManager.Dispose = new Action(() => { });
        }

        private static void AddEntityComponent<TComponent>(TComponent component, int entityId) where TComponent : struct
        {
            int entityIndex = Array.IndexOf(ComponentData<TComponent>.EntityIdReferences, entityId);

            if (entityIndex < 0)
            {
                if (!ComponentData<TComponent>.EntityIdReferences.Contains(-1))
                {
                    ArrayUtils.ResizeAndFillArray(ref ComponentData<TComponent>.EntityIdReferences,
                        ComponentData<TComponent>.EntityIdReferences.Length + 1,
                        -1);
                }

                entityIndex = Array.IndexOf(ComponentData<TComponent>.EntityIdReferences, -1);

                if (ComponentData<TComponent>.EntityComponents.Length < entityIndex + 1)
                    Array.Resize(ref ComponentData<TComponent>.EntityComponents, entityIndex + 1);
            }

            ComponentData<TComponent>.EntityComponents[entityIndex] = component;
            ComponentData<TComponent>.EntityIdReferences[entityIndex] = entityId;
        }

        private static void AddOrReplaceEntityComponent<TComponent>(TComponent component, int entityId) where TComponent : struct
        {
            if (entityId < 0)
                throw new Exception("Entity id must be more than 0!");

            if (HasComponent<TComponent>(entityId))
            {
                var index = GetEntityComponentIndex<TComponent>(entityId);
                ComponentData<TComponent>.EntityComponents[index] = component;
                return;
            }

            AddEntityComponent(component, entityId);
        }

        private static void EntityCheck(int entityId)
        {
            if (!EntityManager.EntityExists(entityId))
                throw new Exception("Entity intialized incorrectly, make sure to create entities using SystemManager.CreateEntry().");
        }

        private static void EntityExists(IModEntity entity)
        {
            if (!EntityManager.EntityExists(entity))
                throw new Exception("Entity intialized incorrectly, make sure to create entities using SystemManager.CreateEntry().");
        }

        private static int GetEntityComponentIndex<TComponent>(int entityId) where TComponent : struct
        {
            if (!ComponentData<TComponent>.EntityIdReferences.Contains(entityId))
                return -1;

            return Array.IndexOf(ComponentData<TComponent>.EntityIdReferences, entityId);
        }

        private static void RemoveComponent<TComponent>(int entityId) where TComponent : struct
        {
            EntityCheck(entityId);

            if (!HasComponent<TComponent>(entityId))
                throw new Exception("Entity has no components of this type!");

            int index = GetEntityComponentIndex<TComponent>(entityId);
            ComponentData<TComponent>.EntityIdReferences[index] = -1;
            ComponentData<TComponent>.EntityComponents[index] = default(TComponent);
        }

        private static class ComponentData<TComponent> where TComponent : struct
        {
            public static TComponent[] EntityComponents;

            public static int[] EntityIdReferences;

            public static TComponent GlobalComponent;

            static ComponentData()
            {
                EntityIdReferences = new int[0];
                EntityComponents = new TComponent[0];
                ComponentDataManager.Dispose += () => { EntityComponents = null; EntityIdReferences = null; };
            }
        }

        private static class ComponentDataManager
        {
            public static Action Dispose;
        }
    }
}