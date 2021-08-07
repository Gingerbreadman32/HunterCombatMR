using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Services;
using HunterCombatMR.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HunterCombatMR.Managers
{
    public sealed class ComponentManager
        : ManagerBase
    {
        private const string EntityComponents = "EntityComponents";
        private const string EntityIdReferences = "EntityIdReferences";
        private static IDictionary<Tuple<Type, Type>, Func<object, object>> _componentListDelegates;
        private static IDictionary<Tuple<Type, Type>, Func<object, object>> _idListDelegates;

        public static ref TComponent GetEntityComponent<TComponent>(in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);
            EntityCheck(entity.Id);
            int idIndex = GetEntityComponentIndex(ComponentData<TComponent>.EntityIdReferences, entity.Id);
            return ref ComponentData<TComponent>.EntityComponents[idIndex];
        }

        public static ref object GetEntityComponent(in IModEntity entity,
            Type componentType)
        {
            var components = GetEntityComponentsGeneric(componentType);
            int idIndex = GetEntityComponentIndex(GetEntityIdsGeneric(componentType), entity.Id);

            return ref components[idIndex];
        }

        public static ref TComponent GetGlobalComponent<TComponent>() where TComponent : struct
        {
            return ref ComponentData<TComponent>.GlobalComponent;
        }

        public static bool HasComponent<TComponent>(in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);
            EntityCheck(entity.Id);
            var idReference = GetEntityComponentIndex(ComponentData<TComponent>.EntityIdReferences, entity.Id);
            return idReference > -1;
        }

        public static bool HasComponent(in IModEntity entity,
            Type componentType)
        {
            EntityExists(entity);
            EntityCheck(entity.Id);
            var idReference = GetEntityComponentIndex(GetEntityIdsGeneric(componentType), entity.Id);
            return idReference > -1;
        }

        public static void RegisterComponent<TComponent>(TComponent component, in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);
            EntityCheck(entity.Id);
            AddOrReplaceEntityComponent(component, entity);
        }

        public static void RemoveComponent<TComponent>(in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);
            EntityCheck(entity.Id);

            if (!HasComponent<TComponent>(entity))
                throw new Exception("Entity has no components of this type!");

            int index = GetEntityComponentIndex(ComponentData<TComponent>.EntityIdReferences, entity.Id);
            ComponentData<TComponent>.EntityIdReferences[index] = -1;
            ComponentData<TComponent>.EntityComponents[index] = default(TComponent);
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

        private static void AddOrReplaceEntityComponent<TComponent>(TComponent component, in IModEntity entity) where TComponent : struct
        {
            EntityExists(entity);
            EntityCheck(entity.Id);

            if (HasComponent<TComponent>(entity))
            {
                var index = GetEntityComponentIndex(ComponentData<TComponent>.EntityIdReferences, entity.Id);
                ComponentData<TComponent>.EntityComponents[index] = component;
                return;
            }

            AddEntityComponent(component, entity.Id);
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