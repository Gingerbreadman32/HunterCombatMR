using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
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

        [TriggerParameter(CommonTriggerParams.FrameTime)]
        public int CurrentFrame { get => Animator.Frame; }

        [TriggerParameter(CommonTriggerParams.KeyframeTime)]
        public int CurrentKeyFrame { get => Animator.GetCurrentKeyFrame(); }

        public int LastFrame { get => Animator.GetFinalFrame(); }
    }
}