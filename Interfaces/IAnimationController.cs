using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;

namespace HunterCombatMR.Interfaces
{
    public interface IAnimationController
    {
        Animator Animator { get; }
        ICustomAnimationV2 CurrentAnimation { get; }
    }
}