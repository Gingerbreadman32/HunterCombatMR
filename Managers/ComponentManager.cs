using HunterCombatMR.Attributes;
using HunterCombatMR.Services;
using HunterCombatMR.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HunterCombatMR.Managers
{
    [ManagerDependency(typeof(EntityManager))]
    public sealed class ComponentManager
        : ManagerBase
    {
        private const string EntityComponents = "EntityComponents";
        private const string EntityIdReferences = "EntityIdReferences";
        private static IDictionary<Tuple<Type, Type>, Func<object, object>> _componentListDelegates;
        private static IDictionary<Tuple<Type, Type>, Func<object, object>> _idListDelegates;

        public static ref TComponent GetEntityComponent<TComponent>(int entityId) where TComponent : struct
        {
            EntityExists(entityId);
            int idIndex = GetEntityComponentIndex(ComponentData<TComponent>.EntityIdReferences, entityId);
            return ref ComponentData<TComponent>.EntityComponents[idIndex];
        }

        public static ref object GetEntityComponent(int entityId,
            Type componentType)
        {
            var components = GetEntityComponentsGeneric(componentType);
            int idIndex = GetEntityComponentIndex(GetEntityIdsGeneric(componentType), entityId);

            return ref components[idIndex];
        }

        public static ref TComponent GetGlobalComponent<TComponent>() where TComponent : struct
            => ref ComponentData<TComponent>.GlobalComponent;

        public static bool HasComponent<TComponent>(int entityId) where TComponent : struct
            => CheckEntityComponent(entityId, ComponentData<TComponent>.EntityIdReferences, typeof(TComponent));

        public static bool HasComponent(int entityId,
            Type componentType)
            => CheckEntityComponent(entityId, GetEntityIdsGeneric(componentType), componentType);

        public static void RegisterComponent<TComponent>(TComponent component, int entityId) where TComponent : struct
        {
            EntityExists(entityId);
            EntityManager.AddComponentTypeToEntity(entityId, typeof(TComponent));

            try
            {
                if (HasComponent<TComponent>(entityId))
                {
                    var index = GetEntityComponentIndex(ComponentData<TComponent>.EntityIdReferences, entityId);
                    ComponentData<TComponent>.EntityComponents[index] = component;
                    return;
                }

                AddEntityComponent(component, entityId);
            }
            catch (Exception ex)
            {
                EntityManager.RemoveComponentTypeFromEntity(entityId, typeof(TComponent));
                throw ex;
            }
        }

        public static void RemoveComponent<TComponent>(int entityId) where TComponent : struct
        {
            EntityExists(entityId);

            if (!HasComponent<TComponent>(entityId))
                throw new Exception("Entity has no components of this type!");

            int index = GetEntityComponentIndex(ComponentData<TComponent>.EntityIdReferences, entityId);
            ComponentData<TComponent>.EntityIdReferences[index] = -1;
            ComponentData<TComponent>.EntityComponents[index] = default(TComponent);
            EntityManager.RemoveComponentTypeFromEntity(entityId, typeof(TComponent));
        }

        public static void RemoveComponent(int entityId,
            Type componentType)
        {
            EntityExists(entityId);

            if (!HasComponent(entityId, componentType))
                throw new Exception("Entity has no components of this type!");

            var idRefs = GetEntityIdsGeneric(componentType);
            var components = GetEntityComponentsGeneric(componentType);
            int index = GetEntityComponentIndex(idRefs, entityId);
            idRefs[index] = -1;
            components[index] = Activator.CreateInstance(componentType);
            EntityManager.RemoveComponentTypeFromEntity(entityId, componentType);
        }

        protected override void OnDispose()
        {
            _componentListDelegates = null;
            _idListDelegates = null;
            ComponentDataManager.Dispose?.Invoke();
        }

        protected override void OnInitialize()
        {
            _componentListDelegates = new Dictionary<Tuple<Type, Type>, Func<object, object>>();
            _idListDelegates = new Dictionary<Tuple<Type, Type>, Func<object, object>>();

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
                    var temp = ComponentData<TComponent>.EntityIdReferences;
                    ArrayUtils.ResizeAndFillArray(ref temp,
                        ComponentData<TComponent>.EntityIdReferences.Length + 1,
                        -1);
                    ComponentData<TComponent>.EntityIdReferences = temp;
                }

                entityIndex = Array.IndexOf(ComponentData<TComponent>.EntityIdReferences, -1);

                if (ComponentData<TComponent>.EntityComponents.Length < entityIndex + 1)
                {
                    var temp = ComponentData<TComponent>.EntityComponents;
                    Array.Resize(ref temp, entityIndex + 1);
                    ComponentData<TComponent>.EntityComponents = temp;
                }
            }

            ComponentData<TComponent>.EntityComponents[entityIndex] = component;
            ComponentData<TComponent>.EntityIdReferences[entityIndex] = entityId;
        }

        private static bool CheckEntityComponent(int entityId,
            int[] entityReferences,
            Type componentType)
        {
            EntityExists(entityId);
            bool hasComponent = GetEntityComponentIndex(entityReferences, entityId) > -1;
            UpdateEntityComponentTypes(entityId, hasComponent, componentType);
            return hasComponent;
        }

        private static bool EntityExists(int entityId)
        {
            if (!EntityManager.EntityExists(entityId))
                throw new Exception("Entity intialized incorrectly, make sure to create entities using SystemManager.CreateEntry().");

            return true;
        }

        private static int GetEntityComponentIndex(int[] ids,
            int entityId)
        {
            if (!ids.Contains(entityId))
                return -1;

            return Array.IndexOf(ids, entityId);
        }

        private static object[] GetEntityComponentsGeneric(Type componentType)
        {
            var getMethod = GetOrAddDelegate(componentType, _componentListDelegates, EntityComponents);
            var components = getMethod.Invoke(null) as IList;
            return components.Cast<object>().ToArray();
        }

        private static int[] GetEntityIdsGeneric(Type componentType)
        {
            var getMethod = GetOrAddDelegate(componentType, _idListDelegates, EntityIdReferences);

            return (int[])getMethod.Invoke(null);
        }

        private static Func<object, object> GetOrAddDelegate(Type componentType,
            IDictionary<Tuple<Type, Type>, Func<object, object>> delegateList,
            string propertyName)
        {
            Tuple<Type, Type> typeKey;

            if (TryGetDelegateKey(componentType, delegateList, out typeKey))
                return delegateList[typeKey];

            var generic = typeof(ComponentData<>).MakeGenericType(componentType);
            var getMethodDel = ReflectionUtils.CreateGetMethodDelegate(generic.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static));

            delegateList.Add(new Tuple<Type, Type>(componentType, generic), getMethodDel);
            return getMethodDel;
        }

        private static bool TryGetDelegateKey(Type componentType, IDictionary<Tuple<Type, Type>, Func<object, object>> delegateList,
            out Tuple<Type, Type> key)
        {
            key = delegateList.Keys.SingleOrDefault(x => x.Item1 == componentType);

            return key != null;
        }

        private static void UpdateEntityComponentTypes(int entityId,
            bool hasComponent,
            Type componentType)
        {
            if (hasComponent)
            {
                EntityManager.AddComponentTypeToEntity(entityId, componentType);
                return;
            }

            EntityManager.RemoveComponentTypeFromEntity(entityId, componentType);
        }

        private static class ComponentData<TComponent> where TComponent : struct
        {
            private static TComponent[] entityComponents;
            private static int[] entityIdReferences;
            private static TComponent globalComponent;

            static ComponentData()
            {
                entityIdReferences = new int[0];
                entityComponents = new TComponent[0];
                ComponentDataManager.Dispose += () => { entityComponents = null; entityIdReferences = null; };
            }

            public static TComponent[] EntityComponents { get => entityComponents; set => entityComponents = value; }

            public static int[] EntityIdReferences { get => entityIdReferences; set => entityIdReferences = value; }

            public static ref TComponent GlobalComponent { get => ref globalComponent; }
        }

        private static class ComponentDataManager
        {
            public static Action Dispose;
        }
    }
}