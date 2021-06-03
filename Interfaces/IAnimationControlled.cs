namespace HunterCombatMR.Interfaces
{
    public interface IAnimationControlled<T> where T : IAnimationController
    {
        T AnimationController { get; }
    }
}