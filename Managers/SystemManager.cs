using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Services;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Managers
{
    public sealed class SystemManager
        : ManagerBase
    {
        private static ICollection<IModSystem> _systems;

        public static IEnumerable<IModSystem> Systems { get => _systems; }

        public static void AddSystem(IModSystem system)
        {
            _systems.Add(system);
        }

        public static void PostInputUpdate()
        {
            foreach (var system in _systems)
            {
                system.PostInputUpdate();
            }
        }

        public static void PostUpdateEverything()
        {
            foreach (var system in _systems)
            {
                system.PostEntityUpdate();
            }
        }

        public static void PreUpdateEverything()
        {
            foreach (var system in _systems)
            {
                system.PreEntityUpdate();
            }
        }

        public static bool SendMessage<TMessage>(TMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("Message cannot be null!");

            IModSystem system = GetSystemByMessageType(typeof(TMessage));

            return system.HandleMessage(message);
        }

        internal static IModSystem GetSystemByComponentType(Type componentType)
        {
            var system = _systems.SingleOrDefault(x => x.ComponentType == componentType);
            if (system == null)
                throw new Exception($"No system exists that can handle component of type {componentType.Name}");
            return system;
        }

        internal static IModSystem GetSystemByMessageType(Type messageType)
        {
            var system = _systems.SingleOrDefault(x => x.MessageTypes.Any(y => y == messageType));
            if (system == null)
                throw new Exception($"No system exists that can handle message of type {messageType.Name}");
            return system;
        }

        protected override void OnDispose()
        {
            _systems = null;
        }

        protected override void OnInitialize()
        {
            _systems = new List<IModSystem>(ReflectionUtils.InstatiateTypesFromInterface<IModSystem>());
        }
    }
}