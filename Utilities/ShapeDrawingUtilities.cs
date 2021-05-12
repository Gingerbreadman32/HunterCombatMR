using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HunterCombatMR.Utilities
{
    public static class ShapeDrawingUtilities
    {
        internal static void DrawLine(SpriteBatch spriteBatch,
            Vector2 position,
            float direction,
            float length)
        {
            Texture2D SimpleTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            SimpleTexture.SetData(new[] { Color.White });

            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, 2f);
            spriteBatch.Draw(SimpleTexture, position - Main.screenPosition, null, Color.Red, direction, origin, scale, SpriteEffects.None, 0);
        }
    }
}