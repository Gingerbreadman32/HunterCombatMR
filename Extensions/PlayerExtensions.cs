using Terraria;

namespace HunterCombatMR.Extensions
{
    public static class PlayerExtensions
    {
        #region Public Methods

        public static bool IsPlayerWalking(this Player player)
            => (player.controlLeft || player.controlRight) && player.velocity.X != 0;

        public static bool IsPlayerJumping(this Player player)
            => player.jump > 0 || player.justJumped;

        public static bool IsPlayerAerial(this Player player)
            => player.velocity.Y != 0;

        #endregion Public Methods
    }
}