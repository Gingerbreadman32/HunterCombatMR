using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Models.Entity;
using HunterCombatMR.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Managers
{
    public sealed class EntityManager
        : ManagerBase
    {
        private static ICollection<IModEntity> _entities; // Do I need this?
        private static ICollection<int> _ids;
        private static IDictionary<int, ICollection<Type>> _componentTypes;
        public static IEnumerable<IModEntity> EntityList { get { return _entities; } }
        public static IEnumerable<int> IdList { get { return _ids; } }

        public static IModEntity CreateEntity()
        {
            var id = IdManager.NextID();
            var entity = new ModEntity(id);
            _ids.Add(id);
            _entities.Add(entity);
            return entity;
        }

        public static IReadOnlyList<Type> GetEntityComponentTypes(int id)
        {
            if (!EntityExists(id))
                throw new IndexOutOfRangeException($"No entity exists with id of {id}");

            if (!_componentTypes.ContainsKey(id))
                return new List<Type>();

            return new List<Type>(_componentTypes[id]);
        }

        internal static void AddComponentTypeToEntity(int id,
            Type componentType)
        {
            if (GetEntityComponentTypes(id).Any(x => x.Equals(componentType)))
                return;

            if (!_componentTypes.ContainsKey(id))
                _componentTypes.Add(id, new List<Type>());

            _componentTypes[id].Add(componentType);
        }

        internal static void RemoveComponentTypeFromEntity(int id,
            Type componentType)
        {
            if (!GetEntityComponentTypes(id).Any(x => x.Equals(componentType)))
                return;

            _componentTypes[id].Remove(componentType);

            if (!GetEntityComponentTypes(id).Any())
                _componentTypes.Remove(id);
        }

        public static bool EntityExists(int id)
            => _ids.Contains(id) && _entities.Any(x => x.Id == id);

        public static bool EntityExists(IModEntity entity)
            => _entities.Contains(entity);

        public static IModEntity GetEntity(int id)
        {
            if (!EntityExists(id))
            {
                throw new IndexOutOfRangeException($"No entity exists with id of {id}");
            }

            return _entities.Single(x => x.Id == id);
        }

        public static void RemoveEntity(int id)
        {
            IdManager.Free(id);
            _entities.Remove(GetEntity(id));
            _ids.Remove(id);
            if (_componentTypes.ContainsKey(id))
                _componentTypes.Remove(id);
        }

        protected override void OnDispose()
        {
            _entities = null;
            _ids = null;
            _componentTypes = null;
        }

        protected override void OnInitialize()
        {
            _ids = new List<int>();
            _entities = new List<IModEntity>();
            _componentTypes = new Dictionary<int, ICollection<Type>>();
        }
    }
}