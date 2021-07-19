using HunterCombatMR.Interfaces.Component;
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

        void RegisterComponent<T>(T component, IModEntity entity) where T : class, IModComponent;

        T GetComponent<T>(IModEntity entity) where T : class, IModComponent;

        bool HasComponent(IModEntity entity);

        void RemoveComponent(IModEntity entity);
    }
}