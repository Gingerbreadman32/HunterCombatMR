using HunterCombatMR.Interfaces.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Services.Systems
{
    public abstract class ModSystem
        : IModSystem
    {
        public ModSystem()
        {
            var type = GetType();
            if (!type.GetInterfaces().Any(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IMessageHandler<>)))
                throw new Exception($"System must accept at least one type of message! Make sure {GetType().Name} inherits from at least one IMessageHandler!");
        }

        public bool HandleMessage<TMessage>(TMessage message) where TMessage : struct
        {
            return (this as IMessageHandler<TMessage>).HandleMessage(message);
        }

        public virtual void PostEntityUpdate()
        {
        }

        public virtual void PostInputUpdate()
        {
        }

        public virtual void PreEntityUpdate()
        {
        }

        public IEnumerable<Type> GetMessageTypes()
        {
            var interfaces = GetType().GetInterfaces().Where(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IMessageHandler<>));
            return interfaces.Select(x => x.GetGenericArguments()[0]);
        }
    }
}