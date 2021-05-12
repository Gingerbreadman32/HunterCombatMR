using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;

namespace HunterCombatMR.Models
{
    public class ProjectileAnimation
        : CustomAnimation
    {
        public ProjectileAnimation(string name)
            : base(name)
        {
        }

        public ProjectileAnimation(ProjectileAnimation copy,
            string name)
            : base(name)
        {
            Name = copy.Name;
            LayerData = new LayerData(copy.LayerData);
            IsStoredInternally = copy.IsStoredInternally;
            AnimationData = new Animator();
            Initialize();
        }

        public override AnimationType AnimationType => AnimationType.Projectile;

        public override IHunterCombatContentInstance CloneFrom(string internalName)
            => new ProjectileAnimation(this, internalName);
    }
}