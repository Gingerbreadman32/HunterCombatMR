namespace HunterCombatMR.Interfaces
{
    public interface IAnimatedEntity<T> where T : IAnimationController
    {
        T AnimationController { get; }
    }
}