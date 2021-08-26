using HunterCombatMR.Models.Animation;
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
        /// <param name="frameInfo">The <see cref="LayerData"/> of the current frame of the sprite being drawn</param>
        public static DrawData SetSpriteOrientation(this DrawData value,
            Player drawPlayer,
            SpriteEffects effects,
            Rectangle layerRectangle)
        {
            var flippedHori = false;
            var flippedVert = false;

            if (effects.Equals(SpriteEffects.FlipHorizontally))
                flippedHori ^= true;
            else if (effects.Equals(SpriteEffects.FlipVertically))
                flippedVert ^= true;

            if (drawPlayer.direction != 1)
                flippedHori ^= true;

            if (drawPlayer.gravDir == -1f)
                flippedVert ^= true;

            // Add any other reasons for sprite to be flipped here

            if (flippedHori && flippedVert)
            {
                value.effect = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
                value.position = Vector2.Transform(value.position, Microsoft.Xna.Framework.Matrix.CreateScale(-1f, 1f, 1f));
            }
            else if (flippedVert)
            {
                value.effect = SpriteEffects.FlipVertically;
            }
            else if (flippedHori)
            {
                value.effect = SpriteEffects.FlipHorizontally;
                value.position = Vector2.Transform(value.position, Microsoft.Xna.Framework.Matrix.CreateScale(-1f, 1f, 1f));
            }
            else
            {
                value.effect = SpriteEffects.None;
            }

            return value;
        }
    }
}