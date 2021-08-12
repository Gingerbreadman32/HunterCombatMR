namespace HunterCombatMR.Messages.InputSystem
{
    public struct InputResetMessage
    {
        public InputResetMessage(int entityId)
        {
            EntityId = entityId;
        }

        public int EntityId { get; }
    }
}