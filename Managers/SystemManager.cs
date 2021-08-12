using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Services;
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
            if (!Initialized)
                return;

            foreach (var system in _systems)
            {
                system.PostInputUpdate();
            }
        }

        public static void PostUpdateEverything()
        {
            if (!Initialized)
                return;

            foreach (var system in _systems)
            {
                system.PostEntityUpdate();
            }
        }

        public static void PreUpdateEverything()
        {
            if (!Initialized)
                return;

            foreach (var system in _systems)
            {
                system.PreEntityUpdate();
            }
        }

        public static bool SendMessage<TMessage>(TMessage message) where TMessage : struct
        {
            InitializeCheck();
            IModSystem system = GetSystemByMessageType(typeof(TMessage));

            return system.HandleMessage(message);
        }

        protected override void OnDispose()
        {
            _systems = null;
        }

        protected override void OnInitialize()
        {
            _systems = new List<IModSystem>();
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

        private static void InitializeCheck()
        {
            if (!Initialized)
                throw new Exception($"System Manager not initialized!");
        }
    }
}