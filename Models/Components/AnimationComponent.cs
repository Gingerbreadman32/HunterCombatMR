using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Animation.Entity;

namespace HunterCombatMR.Models.Components
{
    public struct AnimationComponent
    {
        private EntityAnimation _animation;

        public AnimationComponent(EntityAnimation animation)
        {
            _animation = animation;
            Animator = new EntityAnimator(animation);
        }

        public EntityAnimation Animation { get => _animation; set { _animation = value; Animator.Initialize(_animation.Layers.FrameData); } }
        public EntityAnimator Animator { get; }
    }
}