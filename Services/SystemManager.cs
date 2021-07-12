using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Services.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Services
{
    public static class SystemManager
    {
        private static IDictionary<IModSystem, IEnumerable<Type>> _systems;

        public static IEnumerable<IModSystem> Systems { get => _systems?.Keys; }

        public static void Dispose()
        {
            _systems = null;
        }

        public static void Initialize(IEnumerable<IModSystem> systems)
        {
            _systems = new Dictionary<IModSystem, IEnumerable<Type>>();

            foreach (var system in systems)
            {
                _systems.Add(system, system.GetMessageTypes());
            }
        }

        public static void PostInputUpdate()
        {
            if (_systems == null)
                return;

            foreach (var system in _systems)
            {
                system.Key.PostInputUpdate();
            }
        }

        public static bool SendMessage<TMessage>(TMessage message) where TMessage : struct
        {
            var system = _systems.SingleOrDefault(x => x.Value.Any(y => y == typeof(TMessage)));
            if (system.Key == null || system.Value == null)
                throw new Exception($"No system exists that can handle message of type {message.GetType().Name}");

            return system.Key.HandleMessage(message);
        }
    }
}