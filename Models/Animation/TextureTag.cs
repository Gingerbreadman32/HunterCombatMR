﻿using Microsoft.Xna.Framework;

namespace HunterCombatMR.Models.Animation
{
    public struct TextureTag
    {
        public TextureTag(string value,
            Point size)
        {
            Name = value;
            Size = size;
        }

        /// <summary>
        /// The tag name, used as reference.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The size of the layer. All frames of both the spritesheet will need to reflect this
        /// size, so a new layer will need to be made if alterations need to be made.
        /// </summary>
        public Point Size { get; }
    }
}