using HunterCombatMR.Models;
using HunterCombatMR.Models.Action;

namespace HunterCombatMR.Interfaces.Action
{
    public interface ICustomAction<T>
        : IHunterCombatContentInstance
    {
        ActionAnimations Animations { get; }
        string DisplayName { get; }
        ActionEvents<T> Events { get; }

        void ActionLogic(T entity, Animator animator);
    }
}