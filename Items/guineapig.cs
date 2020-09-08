using HunterCombatMR.Projectiles.SwordandShield;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Text;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HunterCombatMR.Items
{
	public class guineapig : ModItem
	{
		private const int LMB1TotalFrames = 26;
		private int[] GroundLMBFrameTimings = new int[7] { 26, 16, 14, 12, 11, 10, 0 };
		private bool nextFrame = true;

		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Example Sword and Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Testing SnS Style Mechanics.");
		}

		public override void SetDefaults() 
		{
			item.damage = 50;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = LMB1TotalFrames;
			item.useAnimation = LMB1TotalFrames;
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

        public override void UseStyle(Player player)
        {
			/*
			if (nextFrame)
			{
				Main.NewText("Current Frame:" + player.itemAnimation.ToString(), new Color(50, 125, 255));
				nextFrame = false;
			}
			*/

			player.itemAnimationMax = LMB1TotalFrames;
			// Windup/Weapon hold for ground LMB, mostly just holding the sword.
            if (player.itemAnimation <= GroundLMBFrameTimings[0] && player.itemAnimation >= GroundLMBFrameTimings[1])
            {
				item.noUseGraphic = false;
				player.itemRotation = MathHelper.ToRadians(-90 * player.direction);
				player.bodyFrame.Y = player.bodyFrame.Height;

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
				CurrentHandOffset += new Vector2(8 * player.direction, 10);

				player.itemLocation = player.position + CurrentHandOffset;
			}
			else if (player.itemAnimation == GroundLMBFrameTimings[1]-1)
            {
				player.bodyFrame.Y = player.bodyFrame.Height * 2;
				Projectile Swipe1 = Projectile.NewProjectileDirect(player.itemLocation, new Vector2(0, 0), mod.GetProjectile("SNSSwipe1").projectile.type, item.damage, item.knockBack, player.whoAmI);
				item.noUseGraphic = true;
			} 
			else if (player.itemAnimation == GroundLMBFrameTimings[2])
            {
				player.bodyFrame.Y = player.bodyFrame.Height * 3;
			} 
			else if (player.itemAnimation == GroundLMBFrameTimings[3])
            {
				player.bodyFrame.Y = player.bodyFrame.Height * 4;
			}
			/*
			else if(player.itemAnimation <= GroundLMBFrameTimings[6])
            {
				
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
				
			}
				*/
			/*
			if (!PlayerInput.Triggers.JustPressed.Up)
			{
				player.itemAnimation++;
			} else
            {
				nextFrame = true;
            }*/
		}
	}
}