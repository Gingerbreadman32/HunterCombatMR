using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace HunterCombatMR.Models.Animation
{
    [DebuggerDisplay("{Alpha} {Position.X} {Position.Y} {Rotation} {SheetFrame} {Orientation.ToString()} {Scale} {Depth}")]
    public struct LayerData
    {
        public LayerData(FrameIndex animFrame,
            FrameIndex sheetNum,
            FrameIndex sheetFrame,
            Point position,
            float rotation,
            float alpha,
            float scale,
            SpriteEffects orientation,
            int depth)
        {
            AnimationKeyframe = animFrame;
            SheetIndex = sheetNum;
            SheetFrame = sheetFrame;
            Position = position;
            Rotation = rotation;
            Alpha = alpha;
            Scale = scale;
            Orientation = orientation;
            Depth = depth;
        }

        public LayerData(float[] data)
        {
            AnimationKeyframe = (int)data[(int)Index.AnimationFrame];
            SheetIndex = (int)data[(int)Index.SheetIndex];
            SheetFrame = (int)data[(int)Index.SheetFrame];
            Position = new Point((int)data[(int)Index.PositionX], (int)data[(int)Index.PositionY]);
            Rotation = data[(int)Index.Rotation];
            Alpha = data[(int)Index.Alpha];
            Scale = data[(int)Index.Scale];
            Orientation = (SpriteEffects)data[(int)Index.Orientation];
            Depth = (int)data[(int)Index.Depth];
        }

        private enum Index
        {
            AnimationFrame = 0,
            SheetIndex = 1,
            SheetFrame = 2,
            PositionX = 3,
            PositionY = 4,
            Rotation = 5,
            Alpha = 6,
            Scale = 7,
            Orientation = 8,
            Depth = 9
        }

        /// <summary>
        /// The amount of transparency the layer will have.
        /// </summary>
        public float Alpha { get; }

        /// <summary>
        /// What frame of the layer's parent animation this data corresponds to.
        /// </summary>
        public int AnimationKeyframe { get; }

        /// <summary>
        /// The layer depth defined by the layer at this frame.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Whether or not the sprite will be flipped horizontally or vertically using <seealso cref="SpriteEffects"/>.
        /// </summary>
        public SpriteEffects Orientation { get; }

        /// <summary>
        /// The position of the layer relative to the center of the player that the layer will be.
        /// </summary>
        public Point Position { get; }

        /// <summary>
        /// The rotation in radians
        /// </summary>
        public float Rotation { get; }

        /// <summary>
        /// The scale of the sprite.
        /// </summary>
        public float Scale { get; }

        /// <summary>
        /// Which frame of the spritesheet the layer will be on. Starting from frame 0 and goes down
        /// from the top of the sheet.
        /// </summary>
        public int SheetFrame { get; }

        /// <summary>
        /// Which sprite sheet to use from the corresponding animation set.
        /// </summary>
        public int SheetIndex { get; }

        public float[] Save()
        {
            var data = new float[10];

            data[(int)Index.AnimationFrame] = AnimationKeyframe;
            data[(int)Index.SheetIndex] = SheetIndex;
            data[(int)Index.SheetFrame] = SheetFrame;
            data[(int)Index.PositionX] = Position.X;
            data[(int)Index.PositionY] = Position.Y;
            data[(int)Index.Rotation] = Rotation;
            data[(int)Index.Alpha] = Alpha;
            data[(int)Index.Scale] = Scale;
            data[(int)Index.Orientation] = (int)Orientation;
            data[(int)Index.Depth] = Depth;

            return data;
        }
    }
}