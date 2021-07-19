using HunterCombatMR.Interfaces.Component;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.Services;
using HunterCombatMR.Interfaces.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Services
{
    public static class SystemManager
    {
        private static IEntityManager _entityManager;
        private static bool _initialized;
        private static IEnumerable<IModSystem> _systems;
        public static bool Initialized { get => _initialized; }
        public static IEnumerable<IModSystem> Systems { get => _systems; }

        public static IModEntity CreateEntity()
        {
            InitializeCheck();
            return _entityManager.CreateEntity();
        }

        public static void Dispose()
        {
            _systems = null;
            _entityManager = null;
            _initialized = false;
        }

        public static TComponent GetComponent<TComponent>(IModEntity entity) where TComponent : class, IModComponent
        {
            InitializeCheck();
            IModSystem system = GetSystemByComponentType(typeof(TComponent));
            EntityCheck(entity);

            if (!HasComponentUnsafe(entity, system))
                throw new Exception("Entity has no components of this type!");

            return system.GetComponent<TComponent>(entity);
        }

        public static bool HasComponent<TComponent>(IModEntity entity) where TComponent : class, IModComponent
        {
            InitializeCheck();
            IModSystem system = GetSystemByComponentType(typeof(TComponent));
            EntityCheck(entity);

            return HasComponentUnsafe(entity, system);
        }

        public static void Initialize(IEnumerable<IModSystem> systems)
        {
            _systems = new List<IModSystem>(systems);
            _entityManager = new EntityManager();
            _initialized = true;
        }

        public static void PostInputUpdate()
        {
            if (!Initialized)
                return;

            foreach (var system in _systems)
            {
                system.PostInputUpdate();
            }
        }

        public static TComponent RegisterComponent<TComponent>(TComponent component, IModEntity entity) where TComponent : class, IModComponent
        {
            InitializeCheck();
            IModSystem system = GetSystemByComponentType(typeof(TComponent));
            EntityCheck(entity);

            system.RegisterComponent(component, entity);
            return component;
        }

        public static TComponent RegisterComponent<TComponent>(IModEntity entity) where TComponent : class, IModComponent, new()
        {
            var component = new TComponent();
            return RegisterComponent(component, entity);
        }

        public static void RemoveComponent<TComponent>(IModEntity entity) where TComponent : class, IModComponent
        {
            InitializeCheck();
            IModSystem system = GetSystemByComponentType(typeof(TComponent));
            EntityCheck(entity);

            if (!HasComponentUnsafe(entity, system))
                throw new Exception("Entity has no components of this type!");

            system.RemoveComponent(entity);
        }

        public static bool SendMessage<TMessage>(TMessage message) where TMessage : struct
        {
            InitializeCheck();
            IModSystem system = GetSystemByMessageType(typeof(TMessage));

            return system.HandleMessage(message);
        }

        public static IModEntity WithComponent<TComponent>(this IModEntity entity) where TComponent : class, IModComponent, new()
        {
            RegisterComponent<TComponent>(entity);

            return entity;
        }

        private static void EntityCheck(IModEntity entity)
        {
            if (!_entityManager.EntityExists(entity))
                throw new Exception("Entity intialized incorrectly, make sure to create entities using SystemManager.CreateEntry().");
        }

        private static IModSystem GetSystemByComponentType(Type componentType)
        {
            var system = _systems.SingleOrDefault(x => x.ComponentType == componentType);
            if (system == null)
                throw new Exception($"No system exists that can handle component of type {componentType.Name}");
            return system;
        }

        private static IModSystem GetSystemByMessageType(Type messageType)
        {
            var system = _systems.SingleOrDefault(x => x.MessageTypes.Any(y => y == messageType));
            if (system == null)
                throw new Exception($"No system exists that can handle message of type {messageType.Name}");
            return system;
        }

        private static bool HasComponentUnsafe(IModEntity entity, IModSystem system)
        {
            return system.HasComponent(entity);
        }

        private static void InitializeCheck()
        {
            if (!Initialized)
                throw new Exception("System manager not initialized!");
        }
    }
}