using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.Items
{
    public abstract class WeaponBase
        : ModItem
    {
        #region Public Properties

        public abstract string MoveSet { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        public override bool CanRightClick()
            => true;

        public override bool CanUseItem(Player player)
            => false;

        public override bool ConsumeItem(Player player)
            => false;

        public virtual void Equip(HunterCombatPlayer player)
        {
            Main.NewText($"{item.Name} Equipped");

            player.EquippedWeapon = this;
        }

        public override void RightClick(Player player)
        {
            var modPlayer = player.GetModPlayer<HunterCombatPlayer>();

            if (modPlayer.EquippedWeapon != this)
                Equip(modPlayer);
            else
                UnEquip(modPlayer);

            modPlayer.InputBuffers.ResetBuffers();
        }

        public virtual void UnEquip(HunterCombatPlayer player)
        {
            Main.NewText($"{item.Name} Unequipped");

            player.EquippedWeapon = null;
        }

        #endregion Public Methods
    }
}