namespace HunterCombatMR.Messages.EntityStateSystem
{
    public struct ChangeStateMessage
    {
        public ChangeStateMessage(int entityId,
            int stateNum)
        {
            EntityId = entityId;
            StateNumber = stateNum;
        }

        public int EntityId { get; }

        public int StateNumber { get; }
    }
}