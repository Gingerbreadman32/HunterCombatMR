using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HunterCombatMR.Items
{
    public class guineapig 
        : SwordAndShieldBase
    {

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
            return false;
        }
    }
}