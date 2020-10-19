using HunterCombatMR.AttackEngine.Attacks.SwordandShield;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HunterCombatMR.Items
{
    public class guineapig : ModItem
    {
        private AttackSequence Sequence;

        protected void InitilizeAttacks(Player player)
        {
            var Attacks = new List<string>() { "SNS-LMB1", "SNS-LMB2" };
            Sequence = new AttackSequence("SNS-LMB1", Attacks, player, item);
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
            if (base.CanUseItem(player))
            {
                InitilizeAttacks(player);
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