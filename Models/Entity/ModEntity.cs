using HunterCombatMR.Interfaces.Entity;

namespace HunterCombatMR.Models.Entity
{
    public class ModEntity
        : IModEntity
    {
        public ModEntity(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}