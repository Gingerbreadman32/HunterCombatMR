using HunterCombatMR.Interfaces.Component;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.Services;
using HunterCombatMR.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Services
{
    public class EntityManager
        : IEntityManager
    {
        private readonly IIdManager _idManager = new IdManager();
        private readonly ICollection<int> _ids = new List<int>();
        private ICollection<IModEntity> _entities = new List<IModEntity>();

        public IEnumerable<IModEntity> EntityList { get { return _entities; } }
        public IEnumerable<int> IdList { get { return _ids; } }

        public IModEntity CreateEntity()
        {
            var id = _idManager.NextID();
            var entity = new ModEntity(id);
            _ids.Add(id);
            _entities.Add(entity);
            return entity;
        }

        public bool EntityExists(int id)
            => _ids.Contains(id) && _entities.Any(x => x.Id == id);

        public bool EntityExists(IModEntity entity)
            => _entities.Contains(entity);

        public IModEntity GetEntity(int id)
        {
            if (!EntityExists(id))
            {
                throw new IndexOutOfRangeException($"No entity exists with id of {id}");
            }

            return _entities.Single(x => x.Id == id);
        }

        public void RemoveEntity(int id)
        {
            _entities.Remove(GetEntity(id));
            _ids.Remove(id);
            _idManager.Free(id);
        }
    }
}