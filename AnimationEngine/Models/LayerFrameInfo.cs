using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterCombatMR.AnimationEngine.Models
{
    public struct LayerFrameInfo
    {
        /// <summary>
        /// General use constructor
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        /// <param name="rotation"></param>
        /// <param name="flip"></param>
        /// <param name="sheet"></param>
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
            LayerDepth = 255;
        }

        /// <summary>
        /// Copy constructor, used in <see cref="AnimationLayer"/>.
        /// </summary>
        /// <param name="copy"></param>
        /// <param name="depth"></param>
        public LayerFrameInfo(LayerFrameInfo copy,
            byte depth)
        {
            SpriteFrame = copy.SpriteFrame;
            Position = copy.Position;
            Rotation = copy.Rotation;
            LayerDepthOverride = copy.LayerDepthOverride;
            SheetNameOverride = copy.SheetNameOverride;
            SpriteOrientation = copy.SpriteOrientation;
            LayerDepth = depth;
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
        /// Replaces the LayerDepth if not null.
        /// </summary>
        public byte? LayerDepthOverride { get; set; }

        /// <summary>
        /// At what depth the layer will be drawn, with 0 being in front and 255 being in back.
        /// </summary>
        public byte LayerDepth { get; }

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