using Terraria;

namespace HunterCombatMR.Interfaces
{
    public interface IEntityHolder<T> where T
        : Entity
    {
        T EntityContainer { get; }
    }
}