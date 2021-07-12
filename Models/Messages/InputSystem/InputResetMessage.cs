namespace HunterCombatMR.Models.Messages.InputSystem
{
    public struct InputResetMessage
    {
        public InputResetMessage(int player)
        {
            Player = player;
        }

        public int Player { get; }
    }
}