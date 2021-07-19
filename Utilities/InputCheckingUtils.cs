using Terraria;

namespace HunterCombatMR.Utilities
{
    public static class InputCheckingUtils
    {
        public static bool VanillaPlayerInputNotAccepted()
            => PlayerInputNotAccepted() || Main.blockInput;

        public static bool PlayerInputNotAccepted()
            => PlayerInputBufferPaused() || Main.ingameOptionsWindow || Main.drawingPlayerChat || Main.editSign || Main.editChest;

        public static bool PlayerInputBufferPaused()
            => NoGameInputExists() || Main.gamePaused || Main.gameInactive || !Main.hasFocus;

        public static bool NoGameInputExists()
            => Main.gameMenu;

        internal const int MaximumBufferedInputs = 60;
    }
}