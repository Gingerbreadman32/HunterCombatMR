using HunterCombatMR.Attributes;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Services;
using System;
using System.Collections.Concurrent;

namespace HunterCombatMR.Managers
{
    public sealed class GameCommandManager
        : ManagerBase
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

        protected override void OnDispose()
        {
            _commandCache = new ConcurrentDictionary<string, GameCommand>();
        }

        protected override void OnInitialize()
        {
            _commandCache = null;
        }
    }
}