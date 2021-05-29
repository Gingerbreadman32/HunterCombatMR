using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HunterCombatMR.Models.Animation
{
    public static class LayerDataParameters
    {
        public const int Alpha = 1;
        public const int DepthOverride = 7;
        public const int Orientation = 6;
        public const int PositionX = 2;
        public const int PositionY = 3;
        public const int Rotation = 4;
        public const int SheetFrame = 5;
        public const int Visible = 0;
    }

    public class LayerData
            : ICompact<string>
    {
        public LayerData(LayerData copy)
        {
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
        public LayerData(string[] infoArray)
        {
            Position = new Vector2(float.Parse(infoArray[LayerDataParameters.PositionX]), float.Parse(infoArray[LayerDataParameters.PositionY]));
            Rotation = float.Parse(infoArray[LayerDataParameters.Rotation]);
            SheetFrame = int.Parse(infoArray[LayerDataParameters.SheetFrame]);
            Orientation = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), infoArray[LayerDataParameters.Orientation]);
            Alpha = float.Parse(infoArray[LayerDataParameters.Alpha]);
            DepthOverride = string.IsNullOrEmpty(infoArray[LayerDataParameters.DepthOverride]) ? null : new int?(int.Parse(infoArray[LayerDataParameters.DepthOverride]));
        }

        public LayerData(LayerFrameInfo legacy)
        {
            DepthOverride = legacy.LayerDepthOverride;
            Position = legacy.Position;
            Rotation = legacy.Rotation;
            SheetFrame = legacy.SpriteFrame;
            Orientation = legacy.SpriteOrientation;
            Alpha = 255f;
        }

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

        /// <summary>
        /// Which frame of the spritesheet the layer will be on. Starting from frame 0 and goes down
        /// from the top of the sheet.
        /// </summary>
        public int SheetFrame { get; set; }

        public string[] Save()
                    => new string[]
            {
                Alpha.ToString(),
                Position.X.ToString(),
                Position.Y.ToString(),
                Rotation.ToString(),
                SheetFrame.ToString(),
                Orientation.ToString(),
                DepthOverride?.ToString() ?? null
            };
    }
}