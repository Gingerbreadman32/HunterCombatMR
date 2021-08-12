using HunterCombatMR.Enumerations;

namespace HunterCombatMR.Messages.EntityStateSystem
{
    public struct SetWorldStatusMessage
    {
        public SetWorldStatusMessage(int entityId,
            EntityWorldStatus status)
        {
            EntityId = entityId;
            Status = status;
        }

        public int EntityId { get; }

        public EntityWorldStatus Status { get; }
    }
}