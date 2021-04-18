using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class ProjectileAnimation
        : Animation
    {
        #region Public Constructors

        public ProjectileAnimation(string name)
            : base(name)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public override AnimationType AnimationType => AnimationType.Projectile;

        #endregion Public Properties
    }
}