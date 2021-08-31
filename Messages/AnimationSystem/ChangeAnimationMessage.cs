using HunterCombatMR.Models.Animation;

namespace HunterCombatMR.Messages.AnimationSystem
{
    public struct ChangeAnimationMessage
    {
        public ChangeAnimationMessage(int entityId,
            CustomAnimation animation)
        {
            EntityId = entityId;
            Animation = animation;
        }

        public CustomAnimation Animation { get; }
        public int EntityId { get; }
    }
}