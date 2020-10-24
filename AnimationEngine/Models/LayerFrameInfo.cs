using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterCombatMR.AnimationEngine.Models
{
    public struct LayerFrameInfo
    {
        public LayerFrameInfo(int frame,
            Vector2 position,
            byte? depth = null,
            float rotation = 0,
            SpriteEffects flip = SpriteEffects.None,
            string sheet = null)
        {
            SpriteFrame = frame;
            Position = position;
            Rotation = rotation;
            LayerDepthOverride = depth;
            SheetNameOverride = sheet;
            SpriteOrientation = flip;
        }

        /// <summary>
        /// Which frame of the spritesheet the layer will be on. Starting from frame 0.
        /// </summary>
        public int SpriteFrame { get; set; }

        /// <summary>
        /// The position relative to the player that the layer will be.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The rotation in radians
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// At what depth the layer will be drawn, with 0 being in front and 255 being in back. This will override the layer's default if set.
        /// </summary>
        public byte? LayerDepthOverride { get; set; }

        /// <summary>
        /// Optional parameter that represents a new spritesheet to use for this layer at the current frame.
        /// </summary>
        public string SheetNameOverride { get; set; }

        /// <summary>
        /// Whether or not the sprite will be flipped horizontally or vertically using <seealso cref="SpriteEffects"/>.
        /// </summary>
        public SpriteEffects SpriteOrientation { get; set; }
    }
}