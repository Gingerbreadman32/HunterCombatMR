using Terraria;

namespace HunterCombatMR.Utilities
{
    public static class TerrariaMainUtils
    {
        public static bool GameInputNotAccepted()
            => Main.gameMenu || Main.gameInactive || Main.gamePaused || !Main.hasFocus
                || Main.ingameOptionsWindow;
    }
}