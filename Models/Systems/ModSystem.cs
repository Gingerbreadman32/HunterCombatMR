using HunterCombatMR.Interfaces.Component;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HunterCombatMR.Models.Systems
{
    public abstract class ModSystem<TComponent>
        : IModSystem
        where TComponent : class, IModComponent
    {
        private IEnumerable<Type> _messageTypes;

        public ModSystem()
        {
            GetMessageTypes();
            _components = new Dictionary<int, TComponent>();
        }

        public Type ComponentType { get => typeof(TComponent); }

        public IEnumerable<Type> MessageTypes { get => _messageTypes; }

        private IDictionary<int, TComponent> _components;

        public IReadOnlyDictionary<int, TComponent> Components { get => new ReadOnlyDictionary<int, TComponent>(_components); }

        public bool HasComponent(IModEntity entity)
            => HasComponent(entity.Id);

        protected bool HasComponent(int entityId)
            => _components.ContainsKey(entityId);

        public void RemoveComponent(IModEntity entity)
        {
            _components.Remove(entity.Id);
        }

        public T GetComponent<T>(IModEntity entity) where T : class, IModComponent
            => (GetComponent(entity.Id) as T);

        protected TComponent GetComponent(int entityId)
            => _components[entityId];

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

        private void AddOrReplaceComponent(TComponent component, IModEntity entity)
        {
            if (HasComponent(entity))
            {
                _components[entity.Id] = component;
                return;
            }

            _components.Add(entity.Id, component);
        }

        private void GetMessageTypes()
        {
            var interfaces = GetType().GetInterfaces().Where(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IMessageHandler<>));
            _messageTypes = interfaces.Select(x => x.GetGenericArguments()[0]);
        }

        public void RegisterComponent<T>(T component, IModEntity entity) where T : class, IModComponent
        {
            AddOrReplaceComponent(component as TComponent, entity);
        }
    }
}