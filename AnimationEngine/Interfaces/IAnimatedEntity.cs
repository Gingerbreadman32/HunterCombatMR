namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimatedEntity<T> where T : IAnimation
    {
        T CurrentAnimation { get; }

        bool SetCurrentAnimation(IAnimation newAnimation,
            bool newFile = false);
    }
}