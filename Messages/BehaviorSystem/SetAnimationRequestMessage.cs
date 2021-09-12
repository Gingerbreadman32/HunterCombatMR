namespace HunterCombatMR.Messages.BehaviorSystem
{
    public struct SetAnimationRequestMessage
    {
        public SetAnimationRequestMessage(int id,
            int animNo)
        {
            EntityId = id;
            AnimationNumber = animNo;
        }

        public int AnimationNumber { get; }
        public int EntityId { get; }
    }
}