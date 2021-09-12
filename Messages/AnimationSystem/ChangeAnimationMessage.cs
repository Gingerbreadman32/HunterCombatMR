using HunterCombatMR.Models.Animation;

namespace HunterCombatMR.Messages.AnimationSystem
{
    public struct ChangeAnimationMessage
    {
        public ChangeAnimationMessage(int entityId,
            CustomAnimation? animation)
        {
            EntityId = entityId;
            NewAnimation = animation;
        }

        public CustomAnimation? NewAnimation { get; }
        public int EntityId { get; }
    }
}