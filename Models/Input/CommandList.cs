using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Input
{
    public struct CommandList
    {
        public CommandList(IEnumerable<Command> commands,
            FrameLength time,
            FrameLength buffer)
        {
            _commands = new List<Command>(commands);
            DefaultBuffer = buffer;
            DefaultTime = time;
        }

        private IEnumerable<Command> _commands;

        public FrameLength DefaultBuffer { get; }

        public FrameLength DefaultTime { get; }

        public Command GetCommand(string name)
        {
            if (!HasCommand(name))
                throw new Exception($"No command with name {name} exists in this list!");

            return _commands.Single(x => x.Name.Equals(name));
        }

        public bool HasCommand(string name)
            => _commands.Any(x => x.Name.Equals(name));
    }
}