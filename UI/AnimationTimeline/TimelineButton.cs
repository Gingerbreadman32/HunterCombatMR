using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace HunterCombatMR.UI.AnimationTimeline
{
    public class TimelineButton
        : UIImage
    {
        #region Private Fields

        private const int _states = 3;
        private int _clickCooldown = -1;
        private Texture2D _texture;
        private bool _active;

        #endregion Private Fields

        #region Public Constructors

        public TimelineButton(Texture2D texture,
            int scale)
            : base(texture)
        {
            _texture = texture;
            ImageScale = scale;
        }

        #endregion Public Constructors

        #region Public Properties

        public int ClickRate { get; set; } = 30;

        public bool IsActive { get; set; }

        public bool IsHeld { get; private set; }

        public Timeline ParentTimeline
        {
            get
            {
                if (Parent.GetType().IsAssignableFrom(typeof(Timeline)))
                    return Parent as Timeline;
                else
                    return null;
            }
        }

        public delegate void ClickAction();

        public event ClickAction ClickActionEvent;

        #endregion Public Properties

        #region Public Methods

        public override void OnInitialize()
        {
            base.OnInitialize();
            OnMouseDown += (e, l) => { IsHeld = true; };
            OnMouseUp += (e, l) => { IsHeld = false; };
            OnClick += (evt, list) =>
            {
                if (_active)
                {
                    _active = false;
                    ClickActionEvent();
                    StartCooldown();
                }
            };
        }

        public override void Recalculate()
        {
            base.Recalculate();
            Width.Set(_texture.Width * ImageScale, 0f);
            Height.Set((_texture.Height / _states) * ImageScale, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_clickCooldown > 0)
            {
                _active = false;
                _clickCooldown--;
            }
            else if (_clickCooldown == 0)
            {
                if (IsActive)
                    _active = true;

                _clickCooldown = -1;
            }
            else if (_clickCooldown == -1)
            {
                if (IsActive)
                    _active = true;
            }
        }

        public void StartCooldown()
        {
            _clickCooldown = ClickRate;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // Change it so all of my ui draws in the same batch instead of doing this.
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            int height = _texture.Height / _states;
            var source = new Rectangle(0, 0, _texture.Width, height);

            if (!IsHeld && _active && IsMouseHovering)
                source.Y += height;
            else if (!_active || IsHeld)
                source.Y += height * 2;

            spriteBatch.Draw(position: GetDimensions().Position(),
                texture: _texture,
                sourceRectangle: source,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: ImageScale,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
        }

        #endregion Protected Methods
    }
}