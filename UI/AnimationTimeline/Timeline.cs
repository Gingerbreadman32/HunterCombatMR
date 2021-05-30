using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;
using HunterCombatMR.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.UI;

namespace HunterCombatMR.UI.AnimationTimeline
{
    public partial class Timeline
        : UIElement
    {
        private const int _fullHeight = 44;
        private const int _maxFrames = 1024;
        private const int _segmentHeight = 18;
        private const int _segmentWidth = 18;
        private const string _timelineBarEdgePath = UITexturePaths.TimelineTextures + "timelinebaredge";
        private const string _timelineBarMiddlePath = UITexturePaths.TimelineTextures + "timelinebarmiddle";
        private const string _timelineEdgePath = UITexturePaths.TimelineTextures + "timelineedge";
        private const string _timelineMiddlePath = UITexturePaths.TimelineTextures + "timelinemiddle";
        private int _size = 54;

        private Texture2D _timelineEdgeTexture;
        private Texture2D _timelineMidTexture;

        public Timeline(int scale = 1,
                    bool showAllFrames = false)
        {
            Scale = scale;
            FrameList = new HorizontalUIList<TimelineKeyFrameGroup>() { Width = new StyleDimension(((_maxFrames + 2) * _segmentWidth) * Scale, 0f), Height = new StyleDimension(_segmentHeight * Scale, 0), ListPadding = 0f };
            OverflowHidden = true;
            ShowAllFrames = showAllFrames;
        }

        public ICustomAnimationV2 Animation { get; set; }
        public Animator Animator { get; set; }
        public int Scale { get; }

        /// <summary>
        /// Whether or not to show all of the frames associated with a keyframe or just the keyframe.
        /// </summary>
        public bool ShowAllFrames { get; set; }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Texture2D TimelineBarEdgeTexture { get; set; }
        public Texture2D TimelineBarMidTexture { get; set; }
        internal HorizontalUIList<TimelineKeyFrameGroup> FrameList { get; }
        protected IEnumerable<TimelineButton> Buttons { get; set; }

        public void InitializeAnimation()
        {
            foreach (var keyFrame in Animator.KeyFrames)
            {
                bool HasLayers = Animation.Layers[keyFrame.Key].Any();

                var element = new TimelineKeyFrameGroup(this, (HasLayers) ? FrameType.Keyframe : FrameType.Empty, keyFrame.Key, Scale, keyFrame.Value.FrameLength);

                element.Initialize();
                FrameList.Add(element);
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            _timelineEdgeTexture = ModContent.GetTexture(_timelineEdgePath);
            _timelineMidTexture = ModContent.GetTexture(_timelineMiddlePath);
            TimelineBarEdgeTexture = ModContent.GetTexture(_timelineBarEdgePath);
            TimelineBarMidTexture = ModContent.GetTexture(_timelineBarMiddlePath);
            Append(FrameList);
            InitializeButtons();
        }

        public override void Recalculate()
        {
            base.Recalculate();
            Width.Set(CalculateWidth(), 0f);
            Height.Set(_fullHeight * Scale, 0f);
            RecalculateChildren();
        }

        public void ResetFrames()
        {
            TimelineKeyFrameGroup keyFrame = (TimelineKeyFrameGroup)FrameList._items[Animator.CurrentKeyFrameIndex];

            foreach (TimelineKeyFrameGroup offFrame in FrameList._items.Where(x => x != keyFrame))
            {
                if (offFrame.IsActive)
                    offFrame.DeactivateKeyFrame();
            }
        }

        public void SetAnimation(ICustomAnimationV2 animation)
        {
            FrameList.Clear();
            Animation = animation;

            if (Animation != null)
            {
                InitializeAnimation();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
                SetAnimation(null);

            foreach (TimelineButton button in Buttons)
            {
                button.IsActive = button.CheckCondition(Animation);
            }

            if (Animation != null)
            {
                if (HunterCombatMR.Instance.EditorInstance.AnimationEdited)
                    SetAnimation(Animation);
            }
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            DrawOverride(spriteBatch, false);
            base.DrawChildren(spriteBatch);
            DrawOverride(spriteBatch, true);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawOverride(spriteBatch, false);

            var position = GetDimensions().Position();

            position.X = (int)position.X - (4 * Scale);
            position.Y = (int)position.Y;

            spriteBatch.Draw(TimelineBarEdgeTexture,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0);

            position.X += (4 * Scale);

            Rectangle edgeSource = new Rectangle(0, 0, _timelineEdgeTexture.Width, _timelineEdgeTexture.Height);

            spriteBatch.Draw(_timelineEdgeTexture,
                position,
                edgeSource,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0);

            var timelineRoad = new Rectangle((int)position.X + (_timelineEdgeTexture.Width * Scale), (int)position.Y, (_timelineMidTexture.Width * Scale) * (_size - 1), _timelineMidTexture.Height * Scale);

            spriteBatch.Draw(_timelineMidTexture,
                timelineRoad,
                Color.White);

            var timelineBarRoad = new Rectangle(timelineRoad.X, timelineRoad.Y, timelineRoad.Width, TimelineBarMidTexture.Height * Scale);

            spriteBatch.Draw(TimelineBarMidTexture,
                timelineBarRoad,
                Color.White);

            Rectangle endDestination = new Rectangle(timelineRoad.Width + (_timelineEdgeTexture.Width * Scale), (int)position.Y, _timelineEdgeTexture.Width * Scale, _timelineEdgeTexture.Height * Scale);

            spriteBatch.Draw(_timelineEdgeTexture,
                endDestination,
                edgeSource,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.FlipHorizontally,
                0f);

            var barEdgeDestination = new Rectangle(endDestination.X, endDestination.Y, TimelineBarEdgeTexture.Width * Scale, TimelineBarEdgeTexture.Height * Scale);

            spriteBatch.Draw(TimelineBarEdgeTexture,
                barEdgeDestination,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.FlipHorizontally,
                0f);

            DrawOverride(spriteBatch, true);
        }

        private int CalculateWidth()
            => ((_timelineMidTexture != null) ? TimelineBarMidTexture.Width : 0 * Scale) * _size;

        private void DrawOverride(SpriteBatch spriteBatch,
            bool end)
        {
            if (end)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            }
        }
    }
}