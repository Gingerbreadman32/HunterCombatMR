using HunterCombatMR.AnimationEngine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    public class Timeline
        : UIElement
    {
        #region Private Fields

        private const string _timelineEdge = UITexturePaths.TimelineTextures + "timelineedge";
        private const string _timelineMiddle = UITexturePaths.TimelineTextures + "timelinemiddle";
        private const int _segmentSize = 10;
        private int _size = 8;
        private int _segmentHeight = 16;

        #endregion Private Fields

        public UIList FrameList { get; }
        public AnimationEngine.Models.Animation Animation { get; protected set; }

        public Timeline(IEnumerable<KeyFrame> frames,
            AnimationEngine.Models.Animation animation = null)
        {
            if (animation != null)
                SetAnimation(animation);
            FrameList = new UIList() { Width = StyleDimension.Fill, Height = StyleDimension.Fill };
        }

        public override void OnInitialize()
        {
            Append(FrameList);
        }

        public void SetAnimation(AnimationEngine.Models.Animation animation)
        {
            Animation = animation;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var position = GetDimensions().Position();

            position.X = (int)position.X;
            position.Y = (int)position.Y;

            spriteBatch.Draw(ModContent.GetTexture(_timelineEdge),
                position,
                Color.White);

            var timelineRoad = new Rectangle((int)position.X, (int)position.Y, _segmentSize * _size, _segmentHeight);

            spriteBatch.Draw(ModContent.GetTexture(_timelineEdge),
                position,
                Color.White);

            position.X -= _segmentSize;

            spriteBatch.Draw(ModContent.GetTexture(_timelineEdge),
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.FlipHorizontally,
                0f);
        }
    }
}