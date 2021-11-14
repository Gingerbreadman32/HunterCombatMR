using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
using HunterCombatMR.Models.Animation;

namespace HunterCombatMR.Components
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

        [TriggerParameter(ComponentTriggerParams.FrameTime)]
        public int CurrentFrame { get => Animator.Frame; }

        [TriggerParameter(ComponentTriggerParams.KeyframeTime)]
        public int CurrentKeyFrame { get => Animator.GetCurrentKeyframe(); }

        public int LastFrame { get => Animator.GetFinalFrame(); }
    }
}