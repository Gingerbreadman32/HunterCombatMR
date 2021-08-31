using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
using HunterCombatMR.Models.Animation;

namespace HunterCombatMR.Models.Components
{
    public struct AnimationComponent
    {
        private CustomAnimation _animation;

        public AnimationComponent(CustomAnimation animation)
        {
            _animation = animation;
            Animator = new AnimationDef(animation);
        }

        public CustomAnimation Animation { get => _animation; set { _animation = value; Animator.Initialize(_animation.FrameData, _animation.LoopStyle); } }
        public AnimationDef Animator { get; }

        [TriggerParameter(CommonTriggerParams.FrameTime)]
        public int CurrentFrame { get => Animator.Frame; }

        [TriggerParameter(CommonTriggerParams.KeyframeTime)]
        public int CurrentKeyFrame { get => Animator.GetCurrentKeyFrame(); }

        public int LastFrame { get => Animator.GetFinalFrame(); }
    }
}