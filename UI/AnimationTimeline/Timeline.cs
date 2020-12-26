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

        private const string _timelineEdgePath = UITexturePaths.TimelineTextures + "timelineedge";
        private const string _timelineMiddlePath = UITexturePaths.TimelineTextures + "timelinemiddle";
        private const string _timelineBarEdgePath = UITexturePaths.TimelineTextures + "timelinebaredge";
        private const string _timelineBarMiddlePath = UITexturePaths.TimelineTextures + "timelinebarmiddle";
        private int _segmentHeight = 18;
        private int _fullHeight = 44;
        private int _size = 42;

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

        public Timeline(int scale = 2)
        {
            Scale = scale;
            FrameList = new HorizontalUIList() { Width = StyleDimension.Fill, Height = new StyleDimension(_segmentHeight * Scale, 0), ListPadding = 0f };
            OverflowHidden = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public AnimationEngine.Models.Animation Animation { get; protected set; }
        public int Scale { get; }

        #endregion Public Properties

        #region Internal Properties

        internal HorizontalUIList FrameList { get; }

        #endregion Internal Properties

        #region Public Methods

        public void InitializeAnimation()
        {
            int index = 0;

            foreach (var keyFrame in Animation.AnimationData.KeyFrames)
            {
                bool HasLayers = Animation.LayerData.Layers.Any(x => x.GetActiveAtFrame(index));

                var element = new KeyFrameGroup((HasLayers) ? FrameType.Keyframe : FrameType.Empty, index, Scale, keyFrame.FrameLength);

                element.SelectedAction += () => { Animation.AnimationData.SetKeyFrame(element.KeyFrame); };

                element.Initialize();
                FrameList.Add(element);
                index++;
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
        }

        public void SetAnimation(AnimationEngine.Models.Animation animation)
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
            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
                SetAnimation(null);

            if (Animation != null)
            {
                KeyFrameGroup keyFrame = (KeyFrameGroup)FrameList._items[Animation.AnimationData.GetCurrentKeyFrameIndex()];

                keyFrame.ActivateKeyFrame();

                foreach (KeyFrameGroup offFrame in FrameList._items.Where(x => x != keyFrame))
                {
                    if (offFrame.IsActive)
                        offFrame.DeactivateKeyFrame();
                }

                if (HunterCombatMR.Instance.EditorInstance.AnimationEdited)
                    SetAnimation(Animation);
            }
        }

        public override void Recalculate()
        {
            base.Recalculate();
            Width.Set(CalculateWidth(), 0f);
            Height.Set(_fullHeight * Scale, 0f);
            RecalculateChildren();
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

        private int CalculateWidth()
            => ((_timelineMidTexture != null) ? _timelineBarMidTexture.Width : 0 * Scale) * _size;
        
    }
}