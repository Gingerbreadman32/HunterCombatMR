using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace HunterCombatMR.UI.AnimationTimeline
{
    public class TimelineButton
        : UIElement
    {
        #region Private Fields

        // Constants
        private const int _states = 3;
        private const string _defaultButtonTexturePath = UITexturePaths.TimelineTextures + "timelinebutton";
        // Readonly
        private readonly TimelineButtonIcon[] _flippedIcons = { TimelineButtonIcon.LeftArrow };
        // Fields
        private bool _active;
        private Texture2D _buttonTexture;
        private int _clickCooldown = -1; 
        private Texture2D _iconTexture;
        private TimelineButtonIcon _icon;

        #endregion Private Fields

        #region Public Constructors

        public TimelineButton(string name,
            TimelineButtonIcon icon,
            int scale,
            Texture2D buttonTexture = null)
        {
            Name = name;
            Icon = icon;
            ImageScale = scale;

            if (buttonTexture != null)
                _buttonTexture = buttonTexture;
        }

        #endregion Public Constructors

        #region Public Properties

        public Vector2 ButtonPadding { get; set; } = new Vector2(4, 4);

        public event Action ClickActionEvent;

        public event Func<bool> ActiveConditionEvent; 

        public int ClickRate { get; set; } = 30;

        public TimelineButtonIcon Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                _iconTexture = ModContent.GetTexture(value.GetTexturePath());
            }
        }

        public int ImageScale { get; set; }

        public bool IsActive { get; set; }

        public bool IsHeld { get; private set; }

        public string Name { get; set; }

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

        #endregion Public Properties

        #region Public Methods

        public override void OnInitialize()
        {
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

            if (_buttonTexture == null)
                _buttonTexture = ModContent.GetTexture(_defaultButtonTexturePath);

            _iconTexture = ModContent.GetTexture(Icon.GetTexturePath());

            Width.Set(_buttonTexture.Width * ImageScale, 0f);
            Height.Set((_buttonTexture.Height / _states) * ImageScale, 0f);
        }
        public void StartCooldown()
        {
            _clickCooldown = ClickRate;
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
                _active = IsActive;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 iconOffset = Vector2.Zero;
            int height = _buttonTexture.Height / _states;
            var source = new Rectangle(0, 0, _buttonTexture.Width, height);

            if (!IsHeld && _active && IsMouseHovering)
                source.Y += height;
            else if (!_active || IsHeld)
            {
                source.Y += height * 2;
                iconOffset.Y = 2;
            }

            spriteBatch.Draw(position: GetDimensions().Position(),
                texture: _buttonTexture,
                sourceRectangle: source,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: ImageScale,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            spriteBatch.Draw(position: GetDimensions().Position() + (ButtonPadding * ImageScale) + (iconOffset * ImageScale),
                texture: _iconTexture,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: ImageScale,
                effects: (_flippedIcons.Contains(_icon)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0f);
        }

        #endregion Protected Methods
    }
}