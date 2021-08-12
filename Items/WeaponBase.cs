using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.InputSystem;
using HunterCombatMR.Utilities;
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
            player.StateController.CurrentMoveSet = ContentUtils.GetInstance<MoveSet>(MoveSet);
        }

        public override void RightClick(Player player)
        {
            var modPlayer = player.GetModPlayer<HunterCombatPlayer>();

            if (modPlayer.EquippedWeapon != this)
                Equip(modPlayer);
            else
                Unequip(modPlayer);

            SystemManager.SendMessage(new InputResetMessage(player.whoAmI));
        }

        public virtual void Unequip(HunterCombatPlayer player)
        {
            Main.NewText($"{item.Name} Unequipped");

            player.EquippedWeapon = null;
            player.StateController.CurrentMoveSet = null;
        }

        #endregion Public Methods
    }
}