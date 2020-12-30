using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.ModLoader;
using Terraria.UI;

namespace HunterCombatMR.UI.AnimationTimeline
{
    public class Timeline
        : UIElement
    {
        #region Private Fields

        private const string _addButtonPath = UITexturePaths.TimelineTextures + "addkeyframe";
        private const int _fullHeight = 44;
        private const int _maxFrames = 1024;
        private const int _segmentHeight = 18;
        private const int _segmentWidth = 18;
        private const string _timelineBarEdgePath = UITexturePaths.TimelineTextures + "timelinebaredge";
        private const string _timelineBarMiddlePath = UITexturePaths.TimelineTextures + "timelinebarmiddle";
        private const string _timelineEdgePath = UITexturePaths.TimelineTextures + "timelineedge";
        private const string _timelineMiddlePath = UITexturePaths.TimelineTextures + "timelinemiddle";
        private int _size = 84;

        private Texture2D _timelineEdgeTexture;
        private Texture2D _timelineMidTexture;

        public Texture2D _timelineBarEdgeTexture { get; set; }
        public Texture2D _timelineBarMidTexture { get; set; }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        #endregion Private Fields

        #region Public Constructors

        public Timeline(int scale = 1)
        {
            Scale = scale;
            FrameList = new HorizontalUIList<TimelineKeyFrameGroup>() { Width = new StyleDimension((_maxFrames * _segmentWidth) * Scale, 0f), Height = new StyleDimension(_segmentHeight * Scale, 0), ListPadding = 0f };
            OverflowHidden = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public AnimationEngine.Models.Animation Animation { get; protected set; }
        public int Scale { get; }

        #endregion Public Properties

        #region Internal Properties

        internal HorizontalUIList<TimelineKeyFrameGroup> FrameList { get; }

        #endregion Internal Properties

        #region Public Methods

        public void InitializeAnimation()
        {
            foreach (var keyFrame in Animation.AnimationData.KeyFrames.OrderBy(x => x.KeyFrameOrder))
            {
                bool HasLayers = Animation.LayerData.Layers.Any(x => x.GetActiveAtKeyFrame(keyFrame.KeyFrameOrder));

                var element = new TimelineKeyFrameGroup(this, (HasLayers) ? FrameType.Keyframe : FrameType.Empty, keyFrame.KeyFrameOrder, Scale, keyFrame.FrameLength);

                element.Initialize();
                FrameList.Add(element);
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            _timelineEdgeTexture = ModContent.GetTexture(_timelineEdgePath);
            _timelineMidTexture = ModContent.GetTexture(_timelineMiddlePath);
            _timelineBarEdgeTexture = ModContent.GetTexture(_timelineBarEdgePath);
            _timelineBarMidTexture = ModContent.GetTexture(_timelineBarMiddlePath);
            Append(FrameList);

            var addButton = new TimelineButton(ModContent.GetTexture(_addButtonPath), Scale)
            {
                Top = new StyleDimension(20f * Scale, 0f),
                Left = new StyleDimension(28f, 0)
            };
            addButton.ClickActionEvent += AddButton_ClickActionEvent;

            Append(addButton);

            var copyButton = new TimelineButton(ModContent.GetTexture(_addButtonPath), Scale)
            {
                Top = new StyleDimension(20f * Scale, 0f),
                Left = new StyleDimension(88f, 0)
            };
            copyButton.ClickActionEvent += CopyButton_ClickActionEvent;

            Append(copyButton);
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
            TimelineKeyFrameGroup keyFrame = (TimelineKeyFrameGroup)FrameList._items[Animation.AnimationData.GetCurrentKeyFrameIndex()];

            foreach (TimelineKeyFrameGroup offFrame in FrameList._items.Where(x => x != keyFrame))
            {
                if (offFrame.IsActive)
                    offFrame.DeactivateKeyFrame();
            }
        }

        public void SetAnimation(AnimationEngine.Models.Animation animation)
        {
            FrameList.Clear();
            Animation = animation;

            if (Animation != null)
            {
                InitializeAnimation();

                foreach (TimelineButton button in Elements.Where(x => x.GetType().IsAssignableFrom(typeof(TimelineButton))))
                {
                    button.IsActive = true;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
                SetAnimation(null);

            if (Animation == null || !Animation.IsAnimationInitialized())
            {
                foreach (TimelineButton button in Elements.Where(x => x.GetType().IsAssignableFrom(typeof(TimelineButton))))
                {
                    button.IsActive = false;
                }
                
            }
            else
            {
                if (HunterCombatMR.Instance.EditorInstance.AnimationEdited)
                    SetAnimation(Animation);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            var position = GetDimensions().Position();

            position.X = (int)position.X - (4 * Scale);
            position.Y = (int)position.Y;

            spriteBatch.Draw(_timelineBarEdgeTexture,
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

            var timelineRoad = new Rectangle((int)position.X + (_timelineEdgeTexture.Width * Scale), (int)position.Y, CalculateWidth(), _timelineMidTexture.Height * Scale);

            spriteBatch.Draw(_timelineMidTexture,
                timelineRoad,
                Color.White);

            var timelineBarRoad = new Rectangle(timelineRoad.X, timelineRoad.Y, timelineRoad.Width, _timelineBarMidTexture.Height * Scale);

            spriteBatch.Draw(_timelineBarMidTexture,
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

            var barEdgeDestination = new Rectangle(endDestination.X, endDestination.Y, _timelineBarEdgeTexture.Width * Scale, _timelineBarEdgeTexture.Height * Scale);

            spriteBatch.Draw(_timelineBarEdgeTexture,
                barEdgeDestination,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.FlipHorizontally,
                0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddButton_ClickActionEvent()
        {
            Animation.AddKeyFrame();
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private int CalculateWidth()
            => ((_timelineMidTexture != null) ? _timelineBarMidTexture.Width : 0 * Scale) * _size;

        private void CopyButton_ClickActionEvent()
        {
            var copyKeyframe = Animation.AnimationData.GetCurrentKeyFrame();
            var layers = Animation.LayerData.Layers.ToDictionary(x => x, x => x.KeyFrames[copyKeyframe.KeyFrameOrder]);
            Animation.AddKeyFrame(copyKeyframe, layers);
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        #endregion Private Methods
    }
}