using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;

namespace HunterCombatMR.Models
{
    public struct FrameInfo
    {
        

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

        /// <summary>
        /// Whether or not the sprite is enabled for this frame. Hides if false.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}