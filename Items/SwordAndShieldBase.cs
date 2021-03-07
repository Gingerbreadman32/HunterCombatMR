using HunterCombatMR.AttackEngine.Constants;
using Terraria.ModLoader;

namespace HunterCombatMR.Items
{
    public abstract class SwordAndShieldBase
        : ModItem
    {
        #region Public Properties

        public string MoveSet { get; set; } = MoveSetNames.SwordAndShield;

        #endregion Public Properties
    }
}