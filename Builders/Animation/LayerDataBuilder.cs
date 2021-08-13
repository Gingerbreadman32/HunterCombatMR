using HunterCombatMR.Models.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterCombatMR.Builders.Animation
{
    public sealed class LayerDataBuilder
    {
        /// <summary>
        /// The amount of transparency the layer will have.
        /// </summary>
        public float Alpha { get; set; } = 255f;

        /// <summary>
        /// Overrides the layer depth defined by the layer at this frame. Leave null if the normal
        /// depth should be used.
        /// </summary>
        public int? DepthOverride { get; set; }

        /// <summary>
        /// Whether or not the sprite will be flipped horizontally or vertically using <seealso cref="SpriteEffects"/>.
        /// </summary>
        public SpriteEffects Orientation { get; set; }

        /// <summary>
        /// The position of the layer relative to the center of the player that the layer will be.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The rotation in radians
        /// </summary>
        public float Rotation { get; set; }

        public float Scale { get; set; } = 1f;

        /// <summary>
        /// Which frame of the spritesheet the layer will be on. Starting from frame 0 and goes down
        /// from the top of the sheet.
        /// </summary>
        public int SheetFrame { get; set; }

        public LayerData Build()
        {
            return new LayerData(DepthOverride, Position, Rotation, SheetFrame, Orientation, Alpha, Scale);
        }
    }
}