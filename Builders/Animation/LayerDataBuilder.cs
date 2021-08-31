using HunterCombatMR.Models.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterCombatMR.Builders.Animation
{
    public sealed class LayerDataBuilder
    {
        public LayerDataBuilder()
        { }

        public LayerDataBuilder(LayerDataBuilder data)
        {
            AnimationKeyframe = data.AnimationKeyframe;
            SheetIndex = data.SheetIndex;
            SheetFrame = data.SheetFrame;
            Position = data.Position;
            Rotation = data.Rotation;
            Alpha = data.Alpha;
            Scale = data.Scale;
            Orientation = data.Orientation;
            Depth = data.Depth;
        }

        public LayerDataBuilder(LayerData data)
        {
            AnimationKeyframe = data.AnimationKeyframe;
            SheetIndex = data.SheetIndex;
            SheetFrame = data.SheetFrame;
            Position = data.Position;
            Rotation = data.Rotation;
            Alpha = data.Alpha;
            Scale = data.Scale;
            Orientation = data.Orientation;
            Depth = data.Depth;
        }

        /// <summary>
        /// The amount of transparency the layer will have.
        /// </summary>
        public float Alpha { get; set; } = 255f;

        /// <summary>
        /// What frame of the layer's parent animation this data corresponds to.
        /// </summary>
        public int AnimationKeyframe { get; set; }

        /// <summary>
        /// The layer depth defined by the layer at this frame.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Whether or not the sprite will be flipped horizontally or vertically using <seealso cref="SpriteEffects"/>.
        /// </summary>
        public SpriteEffects Orientation { get; set; }

        /// <summary>
        /// The position of the layer relative to the center of the player that the layer will be.
        /// </summary>
        public Point Position { get; set; } = Point.Zero;

        /// <summary>
        /// The rotation in radians
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// The scale of the sprite.
        /// </summary>
        public float Scale { get; set; } = 1f;

        /// <summary>
        /// Which frame of the spritesheet the layer will be on. Starting from frame 0 and goes down
        /// from the top of the sheet.
        /// </summary>
        public int SheetFrame { get; set; }

        /// <summary>
        /// Which sprite sheet to use from the corresponding animation set.
        /// </summary>
        public int SheetIndex { get; set; }

        public LayerData Build()
        {
            return new LayerData(AnimationKeyframe, SheetIndex, SheetFrame, Position, Rotation, Alpha, Scale, Orientation, Depth);
        }
    }
}