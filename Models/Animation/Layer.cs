using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;

namespace HunterCombatMR.Models.Animation
{
    public class Layer
        : IKeyframeDataReference
    {
        [JsonConstructor]
        public Layer(string name,
            int depth,
            TextureTag tag)
        {
            ReferenceName = name ?? throw new ArgumentException("Layer name must be specified!");
            Depth = depth;
            Tag = tag;
        }

        public Layer(Layer copy)
        {
            ReferenceName = copy.ReferenceName;
            Depth = copy.Depth;
            Tag = copy.Tag;
        }

        public Layer(AnimationLayer legacyLayer)
        {
            ReferenceName = legacyLayer.DisplayName;
            Depth = legacyLayer.DefaultDepth;
            Tag = new TextureTag(legacyLayer.DisplayName, new Point(legacyLayer.SpriteFrameRectangle.Width, legacyLayer.SpriteFrameRectangle.Height));
        }

        /// <summary>
        /// At what depth the layer will be drawn, with lower depth being in front.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Name of the layer
        /// </summary>
        public string ReferenceName { get; set; }

        /// <summary>
        /// A tag indicating the type of texture that can be applied to this layer. Defined by name and restricted by size.
        /// </summary>
        public TextureTag Tag { get; set; }
    }
}