using HunterCombatMR.Interfaces.Entity;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces.System
{
    public interface IModSystem
    {
        Type ComponentType { get; }

        IEnumerable<Type> MessageTypes { get; }

        bool HandleMessage<TMessage>(TMessage message) where TMessage : struct;

        void PostEntityUpdate();

        void PostInputUpdate();

        void PreEntityUpdate();
    }
}