using HunterCombatMR.AnimationEngine.Models;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimated<T> where T : Animation
    {
        T CurrentAnimation { get; }

        bool SetCurrentAnimation(T newAnimation,
            bool newFile = false);
    }
}