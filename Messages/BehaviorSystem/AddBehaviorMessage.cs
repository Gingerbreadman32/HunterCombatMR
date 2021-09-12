using HunterCombatMR.Models.Behavior;

namespace HunterCombatMR.Messages.BehaviorSystem
{
    public struct AddBehaviorMessage
    {
        public AddBehaviorMessage(int entityId,
            Behavior behavior,
            int index = -1)
        {
            EntityId = entityId;
            Behavior = behavior;
            Index = index;
        }

        public Behavior Behavior { get; }

        public int EntityId { get; }

        public int Index { get; }
    }
}