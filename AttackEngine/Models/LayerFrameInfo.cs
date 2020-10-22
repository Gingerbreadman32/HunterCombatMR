using Microsoft.Xna.Framework;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct LayerFrameInfo
    {
        public LayerFrameInfo(int frame,
            Vector2 position,
            Rectangle size,
            string name,
            float rotation = 0,
            byte depth = 1,
            string sheet = null)
        {
            SpriteFrame = frame;
            Position = position;
            FrameRectangle = size;
            Name = name;
            Rotation = rotation;
            LayerDepth = depth;
            SheetNameOverride = sheet;
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
        /// At what depth the layer will be drawn, with 0 being in front and 255 being in back.
        /// </summary>
        public byte LayerDepth { get; set; }

        /// <summary>
        /// Optional parameter that represents a new spritesheet to use for this layer at the current frame.
        /// </summary>
        public string SheetNameOverride { get; set; }

        /// <summary>
        /// The name of the layer, will be used if the override is empty or null for texture loading and is used
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size and offset of the frame being used. Leave x and y at 0, 0 if the sprite starts at the top-left.
        /// </summary>
        public Rectangle FrameRectangle { get; set; }
    }
}