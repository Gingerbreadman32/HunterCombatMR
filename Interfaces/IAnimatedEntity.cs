namespace HunterCombatMR.Interfaces
{
    public interface IAnimatedEntity<T> where T : ICustomAnimation
    {
        T CurrentAnimation { get; }

        bool SetCurrentAnimation(ICustomAnimation newAnimation,
            bool newFile = false);
    }
}