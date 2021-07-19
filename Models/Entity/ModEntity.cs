using HunterCombatMR.Interfaces.Component;
using HunterCombatMR.Interfaces.Entity;
using System.Collections.Generic;

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