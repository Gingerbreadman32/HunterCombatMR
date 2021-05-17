using HunterCombatMR.Comparers;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace HunterCombatMR.Models.Animation
{
    public class Layer
        : INamed
    {
        /// <summary>
        /// At what depth the layer will be drawn, with 0 being in front and 255 being in back. 
        /// </summary>
        public byte Depth { get; set; }

        public SortedList<FrameIndex, LayerKeyFrameData> KeyFrameData { get; set; }

        /// <summary>
        /// Name of the layer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size and offset of the first frame the spritesheet being used. Leave x and y at 0, 0 if the sprite starts at the top-left.
        /// Saves the data of the size of each frame, but is transferred to the Texture tuple on load.
        /// </summary>
        /// <remarks>
        /// @@warn Transfer this to a texture object instead so that it can be judge by texture rather than by animation.
        /// </remarks>
        public Rectangle SpriteFrameRectangle { get; set; }

        [JsonIgnore]
        /// <summary>
        /// The texture this layer uses.
        /// </summary>
        public Texture2D Texture { get; protected set; }

        /// <summary>
        /// The path that will be used to load the texture.
        /// </summary>
        public string TexturePath { get; protected set; }


        public bool IsActive(FrameIndex keyFrame)
            => KeyFrameData.ContainsKey(keyFrame) && KeyFrameData[keyFrame].Visible;

        internal void MoveKeyFrame(FrameIndex keyFrameIndex,
            FrameIndex newFrameIndex)
        {
            LayerKeyFrameData temp = new LayerKeyFrameData(KeyFrameData[keyFrameIndex]);
            KeyFrameData[keyFrameIndex] = KeyFrameData[newFrameIndex];
            KeyFrameData[newFrameIndex] = temp;
        }

        internal void RemoveKeyFrame(int keyFrameIndex)
        {
            KeyFrameData.Remove(keyFrameIndex);
            InheritPreviousKeyFrameProperties(keyFrameIndex);
        }

        private void InheritPreviousKeyFrameProperties(int keyFrameIndex)
        {
            int nextFrameIndex = keyFrameIndex + 1;
            if (KeyFrameData.ContainsKey(nextFrameIndex))
            {
                KeyFrameData.Add(keyFrameIndex, KeyFrameData[nextFrameIndex]);
                KeyFrameData.Remove(nextFrameIndex);
                InheritPreviousKeyFrameProperties(nextFrameIndex);
            }
        }
    }
}