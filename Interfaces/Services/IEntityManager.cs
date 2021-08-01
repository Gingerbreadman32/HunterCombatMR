using HunterCombatMR.Interfaces.Entity;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces.Services
{
    public interface IEntityManager
    {
        IEnumerable<IModEntity> EntityList { get; }
        IEnumerable<int> IdList { get; }

        IModEntity CreateEntity();

        bool EntityExists(IModEntity entity);

        bool EntityExists(int id);

        IModEntity GetEntity(int id);

        void RemoveEntity(int id);
    }
}