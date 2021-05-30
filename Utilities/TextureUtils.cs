using HunterCombatMR.Models.Animation;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace HunterCombatMR.Utilities
{
    public static class TextureUtils
    {
        public static Texture2D GetTextureFromTag(TextureTag tag)
        {
            switch (tag.Name)
            {
                case "HC_Head":
                    return ModContent.GetTexture("HunterCombatMR/Textures/SnS/Limbs/HeadFrames");

                case "HC_FrontArm":
                    return ModContent.GetTexture("HunterCombatMR/Textures/SnS/Limbs/FrontArmFrames");

                case "HC_BackArm":
                    return ModContent.GetTexture("HunterCombatMR/Textures/SnS/Limbs/BackArmFrames");

                case "HC_Body":
                    return ModContent.GetTexture("HunterCombatMR/Textures/SnS/Limbs/BodyFrames");

                case "HC_FrontLeg":
                    return ModContent.GetTexture("HunterCombatMR/Textures/SnS/Limbs/FrontLegFrames");

                case "HC_BackLeg":
                    return ModContent.GetTexture("HunterCombatMR/Textures/SnS/Limbs/BackLegFrames");

                case "FullBodyTest":
                    return ModContent.GetTexture("HunterCombatMR/Textures/SnS/Limbs/runningslashtest");

                default:
                    throw new System.ArgumentOutOfRangeException("Tag not implemented!");
            }
        }

        public static int GetTotalTextureFrames(Texture2D texture,
            TextureTag tag)
            => texture.Height / tag.Size.Y;
    }
}