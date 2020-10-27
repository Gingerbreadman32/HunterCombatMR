using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace HunterCombatMR.Extensions
{
    public static class DrawDataExtentions
    {
        /// <summary>
        /// Sets the sprite's orientation based on numerous scenarios that would flip the sprite.
        /// </summary>
        /// <param name="value">The <see cref="DrawData"/> of the sprite being drawn</param>
        /// <param name="drawPlayer">The <see cref="Player"/></param>
        /// <param name="frameInfoEffects">The <see cref="SpriteEffects"/> of the current frame of the sprite being drawn</param>
        /// <remarks>This will not affect position, so make sure that is changed outside of this method</remarks>
        public static DrawData SetSpriteOrientation(this DrawData value,
            Player drawPlayer,
            SpriteEffects frameInfoEffects)
        {
            var flippedHori = false;
            var flippedVert = false;

            if (frameInfoEffects.Equals(SpriteEffects.FlipHorizontally))
                flippedHori ^= true;
            else if (frameInfoEffects.Equals(SpriteEffects.FlipVertically))
                flippedVert ^= true;

            if (drawPlayer.direction != 1)
                flippedHori ^= true;

            if (drawPlayer.gravDir == -1f)
                flippedVert ^= true;

            // Add any other reasons for sprite to be flipped here

            if (flippedHori && flippedVert)
            {
                value.effect = SpriteEffects.None;
                value.rotation += MathHelper.Pi;
            }
            else if (flippedVert)
            {
                value.effect = SpriteEffects.FlipVertically;
            }
            else if (flippedHori)
            {
                value.effect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                value.effect = SpriteEffects.None;
            }

            return value;
        }
    }
}