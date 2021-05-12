using HunterCombatMR.Attributes;
using HunterCombatMR.Enumerations;
using System;
using System.Collections.Concurrent;

namespace HunterCombatMR.Utilities
{
    public static class CachingUtils
    {
        private static ConcurrentDictionary<string, GameCommand> _commandCache;

        public static GameCommand GetGameCommand(string name)
        {
            GameCommand command;

            if (_commandCache.TryGetValue(name, out command))
                return command;

            command = Attribute.GetCustomAttribute(typeof(ActionInputs).GetField(name), typeof(GameCommand)) as GameCommand;
            if (command != null)
                _commandCache.TryAdd(name, command);

            return command;
        }

        internal static void Initialize()
        {
            _commandCache = new ConcurrentDictionary<string, GameCommand>();
        }

        internal static void Uninitialize()
        {
            _commandCache = null;
        }
    }
}