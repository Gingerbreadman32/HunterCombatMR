using System;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces.System
{
    public interface IModSystem
    {
        IEnumerable<Type> GetMessageTypes();

        bool HandleMessage<TMessage>(TMessage message) where TMessage : struct;

        void PostEntityUpdate();

        void PostInputUpdate();

        void PreEntityUpdate();
    }
}