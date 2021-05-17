using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HunterCombatMR.Models.Animation
{
    public struct LayerKeyFrameData
    {
        private const int _arrayLength = 8;

        public LayerKeyFrameData(LayerKeyFrameData copy)
        {
            Visible = copy.Visible;
            DepthOverride = copy.DepthOverride;
            Position = copy.Position;
            Rotation = copy.Rotation;
            SheetFrame = copy.SheetFrame;
            Orientation = copy.Orientation;
            Alpha = copy.Alpha;
        }

        /// <summary>
        /// Initializes a new FrameInfo value using an array of floats for easy save/load.
        /// </summary>
        /// <param name="infoArray">The array of parameters in order.</param>
        public LayerKeyFrameData(float[] infoArray)
        {
            if (infoArray.Length < _arrayLength)
                throw new ArgumentOutOfRangeException($"Frame info array must contain {_arrayLength} parameters.");

            Visible = infoArray[0] > 0;
            DepthOverride = (infoArray[1] > -1) ? (byte?)infoArray[1] : null;
            Position = new Vector2(infoArray[2], infoArray[3]);
            Rotation = infoArray[4];
            SheetFrame = (int)infoArray[5];
            Orientation = (SpriteEffects)((int)infoArray[6]);
            Alpha = infoArray[7];
        }

        /// <summary>
        /// The amount of transparency the layer will have.
        /// </summary>
        public float Alpha { get; }

        /// <summary>
        /// Overrides the layer depth defined by the layer at this frame. Leave null if the normal
        /// depth should be used.
        /// </summary>
        public byte? DepthOverride { get; }

        /// <summary>
        /// Whether or not the sprite will be flipped horizontally or vertically using <seealso cref="SpriteEffects"/>.
        /// </summary>
        public SpriteEffects Orientation { get; }

        /// <summary>
        /// The position of the layer relative to the center of the player that the layer will be.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// The rotation in radians
        /// </summary>
        public float Rotation { get; }

        /// <summary>
        /// Which frame of the spritesheet the layer will be on. Starting from frame 0 and goes down
        /// from the top of the sheet.
        /// </summary>
        public int SheetFrame { get; }

        /// <summary>
        /// Whether or not the sprite is visible for this frame. Hides if false.
        /// </summary>
        public bool Visible { get; }

        public LayerKeyFrameData Default()
            => new LayerKeyFrameData(new float[_arrayLength] { 1f, -1f, 0f, 0f, 0f, 0f, 0f, 255f });

        public float[] Save()
            => new float[_arrayLength]
            {
                (Visible) ? 1f : 0f,
                (DepthOverride.HasValue) ? -1f : DepthOverride.Value,
                Position.X,
                Position.Y,
                Rotation,
                SheetFrame,
                (float)Orientation,
                Alpha
            };
    }
}