using HunterCombatMR.AnimationEngine.Models;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimatedEntity<T> where T : IAnimated
    {
        T CurrentAnimation { get; }

        bool SetCurrentAnimation(T newAnimation,
            bool newFile = false);
    }
}