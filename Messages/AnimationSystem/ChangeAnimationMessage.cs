using HunterCombatMR.Models.Animation;

namespace HunterCombatMR.Messages.AnimationSystem
{
    public struct ChangeAnimationMessage
    {
        public ChangeAnimationMessage(int entityId,
            EntityAnimation animation)
        {
            EntityId = entityId;
            Animation = animation;
        }

        public EntityAnimation Animation { get; }
        public int EntityId { get; }
    }
}