using HunterCombatMR.AttackEngine.Attacks.SwordandShield;
using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HunterCombatMR.Items
{
    public class guineapig : ModItem
    {
        private AttackSequence Sequence;

        private IDictionary<ComboInputs, AttackSequence> Sequences;

        protected void InitilizeAttacks(HunterCombatPlayer player)
        {
            var StartingAttack = new ComboAction("RunningSlash", 
                HunterCombatMR.Instance.GetLoadedAttack("RunningSlash"), 
                new List<ComboRoute>() { new ComboRoute("DoubleSlashFollowup", 
                    DefaultAttackDetails.DefaultBufferWindow,
                    Enumerations.ComboInputs.StandardAttack,
                    0) });
            var FollowUps = new List<ComboAction>() {
                new ComboAction("DoubleSlashFollowup", HunterCombatMR.Instance.GetLoadedAttack("DoubleSlash"), null) 
            };
            Sequence = new AttackSequence(StartingAttack, FollowUps, player, item);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Sword and Shield");
            Tooltip.SetDefault("Testing SnS Style Mechanics.");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 500;
            item.useAnimation = 500;
            item.useStyle = 69;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            //item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.shootSpeed = 1;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            if (base.CanUseItem(player) && (Sequence == null || Sequence.PlayerPeforming.player != player))
            {
                InitilizeAttacks(player.GetModPlayer<HunterCombatPlayer>());
                return true;
            }
            else
            {
                return base.CanUseItem(player);
            }
        }

        public override void UseStyle(Player player)
        {
            if (Sequence != null)
                Sequence.Update();
        }

        /*
				player.itemRotation = MathHelper.ToRadians(56) * player.direction;

				var CurrentHandOffset = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56];
				if (player.direction != 1)
				{
					CurrentHandOffset.X = player.bodyFrame.Width - CurrentHandOffset.X;
				}
				if (player.gravDir != 1f)
				{
					CurrentHandOffset.Y = player.bodyFrame.Height - CurrentHandOffset.Y;
				}
				CurrentHandOffset -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
				CurrentHandOffset += new Vector2(10 * player.direction, 18);

				player.itemLocation = player.position + CurrentHandOffset;
				item.noUseGraphic = false;
				*/
    }
}