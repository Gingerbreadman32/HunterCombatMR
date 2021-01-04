using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        private int _size = 54;

        private Texture2D _timelineEdgeTexture;
        private Texture2D _timelineMidTexture;

        #endregion Private Fields

        #region Public Constructors

        public Timeline(int scale = 1,
                    bool showAllFrames = false)
        {
            Scale = scale;
            FrameList = new HorizontalUIList<TimelineKeyFrameGroup>() { Width = new StyleDimension(((_maxFrames + 2) * _segmentWidth) * Scale, 0f), Height = new StyleDimension(_segmentHeight * Scale, 0), ListPadding = 0f };
            OverflowHidden = true;
            ShowAllFrames = showAllFrames;
        }

        #endregion Public Constructors

        #region Public Properties

        public AnimationEngine.Models.Animation Animation { get; protected set; }

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

        #endregion Public Properties

        #region Internal Properties

        internal HorizontalUIList<TimelineKeyFrameGroup> FrameList { get; }

        #endregion Internal Properties

        #region Public Methods

        public void AddButton(string name,
            TimelineButtonIcon iconType,
            Action buttonEvent)
        {
            var currentButtons = Elements.Where(x => x.GetType().IsAssignableFrom(typeof(TimelineButton))).Count();
            var newButton = new TimelineButton(name, iconType, Scale)
            {
                Top = new StyleDimension(20f * Scale, 0f),
                Left = new StyleDimension((28f * Scale) + (30f * currentButtons * Scale), 0f)
            };
            newButton.ClickActionEvent += buttonEvent;

            Append(newButton);
        }

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
                // @@num:1
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

            if (Animation == null
                || !Animation.IsAnimationInitialized()
                || (!ShowAllFrames && Animation.AnimationData.GetTotalKeyFrames() == _maxFrames)
                || (ShowAllFrames && Animation.AnimationData.TotalFrames == _maxFrames))
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

        #endregion Protected Methods

        #region Private Methods

        private void AddButtonLogic()
        {
            Animation.AddKeyFrame();
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private int CalculateWidth()
            => ((_timelineMidTexture != null) ? TimelineBarMidTexture.Width : 0 * Scale) * _size;

        private void CopyButtonLogic()
        {
            var copyKeyframe = Animation.AnimationData.GetCurrentKeyFrame();
            Animation.AddKeyFrame(copyKeyframe, Animation.LayerData.GetFrameInfoForLayers(copyKeyframe.KeyFrameOrder));
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

        private void DeleteButtonLogic()
        {
            Animation.RemoveKeyFrame(Animation.AnimationData.GetCurrentKeyFrameIndex());
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
        }

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

        private void InitializeButtons()
        {
            AddButton(TimelineButtonNames.AddKeyButton, TimelineButtonIcon.Plus, AddButtonLogic);
            AddButton(TimelineButtonNames.CopyKeyButton, TimelineButtonIcon.Duplicate, CopyButtonLogic);
            AddButton(TimelineButtonNames.RemoveKeyButton, TimelineButtonIcon.Minus, DeleteButtonLogic);
            AddButton(TimelineButtonNames.MoveLeftKeyButton, TimelineButtonIcon.LeftArrow, MoveButtonLogic(false));
            AddButton(TimelineButtonNames.MoveRightKeyButton, TimelineButtonIcon.RightArrow, MoveButtonLogic(true));
        }

        private Action MoveButtonLogic(bool forward)
        {
            return () =>
            {
                int currentKeyFrame = Animation.AnimationData.GetCurrentKeyFrameIndex();
                Animation.MoveKeyFrame(currentKeyFrame, (forward) ? currentKeyFrame + 1 : currentKeyFrame - 1);
                HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
            };
        }

        #endregion Private Methods
    }
}