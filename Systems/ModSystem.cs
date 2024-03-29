﻿using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Systems
{
    public abstract class ModSystem<TComponent>
        : IModSystem
        where TComponent : struct
    {
        private IEnumerable<Type> _messageTypes;

        public ModSystem()
        {
            GetMessageTypes();
            OnCreate();
        }

        public Type ComponentType { get => typeof(TComponent); }

        public IEnumerable<Type> MessageTypes { get => _messageTypes; }

        public ref TComponent GetComponent(in IModEntity entity)
            => ref entity.GetComponent<TComponent>();

        public ref TComponent GetComponent(int Id)
            => ref ComponentManager.GetEntityComponent<TComponent>(Id);

        public bool HandleMessage<TMessage>(TMessage message)
        {
            return (this as IMessageHandler<TMessage>).HandleMessage(message);
        }

        public bool HasComponent(in IModEntity entity)
            => entity.HasComponent<TComponent>();

        public bool HasComponent(int Id)
            => ComponentManager.HasComponent<TComponent>(Id);

        public virtual void PostEntityUpdate()
        {
        }

        public virtual void PostInputUpdate()
        {
        }

        public virtual void PreEntityUpdate()
        {
        }

        public IEnumerable<IModEntity> ReadEntities()
        {
            IEnumerable<IModEntity> entities = new List<IModEntity>();

            return EntityManager.EntityList.Where(x => ComponentManager.HasComponent<TComponent>(x)).Select(x => EntityManager.GetEntity(x));
        }

        protected void ForEachComponentEntity(Action<IModEntity> method)
        {
            foreach (IModEntity entity in ReadEntities())
            {
                method.Invoke(entity);
            }
        }

        protected virtual void OnCreate()
        {
        }

        private void GetMessageTypes()
        {
            var interfaces = GetType().GetInterfaces().Where(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IMessageHandler<>));
            _messageTypes = interfaces.Select(x => x.GetGenericArguments()[0]);
        }
    }
}