using HunterCombatMR.Interfaces.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace HunterCombatMR.Models.UI.Elements
{
    internal class PopUpButton
        : UIPanel,
        IUIAttachable
    {
        protected char _charicon;
        protected Vector2 _distance;
        protected bool _freefloat;
        protected Texture2D _texicon;

        public PopUpButton(char icon,
            float width,
            float height,
            StyleDimension top,
            StyleDimension left)
        {
            _charicon = icon;
            Width.Set(width, 0f);
            Height.Set(height, 0f);
            Left = left;
            Top = top;
            IconColor = Color.White;
        }

        public PopUpButton(Texture2D icon,
            float width,
            float height,
            StyleDimension top,
            StyleDimension left)
        {
            _texicon = icon;
            UseTextureIcon = true;
            Width.Set(width, 0f);
            Height.Set(height, 0f);
            Left = left;
            Top = top;
            IconColor = Color.White;
        }

        public PopUpButton(char icon,
            float width,
            float height,
            UIElement attachedTo,
            Vector2 distanceFromAttach)
        {
            _charicon = icon;
            Width.Set(width, 0f);
            Height.Set(height, 0f);
            Recalculate();
            Attatch(attachedTo, distanceFromAttach);
            IconColor = Color.White;
        }

        public PopUpButton(Texture2D icon,
            float width,
            float height,
            UIElement attachedTo,
            Vector2 distanceFromAttach)
        {
            _texicon = icon;
            UseTextureIcon = true;
            Width.Set(width, 0f);
            Height.Set(height, 0f);
            Recalculate();
            Attatch(attachedTo, distanceFromAttach);
            IconColor = Color.White;
        }

        /// <inheritdoc/>
        public Vector2 AttachDistance
        {
            get
            {
                return _distance;
            }
        }

        /// <inheritdoc/>
        public UIElement AttachedElement { get; protected set; }

        /// <summary>
        /// Character used to represent the icon for this button.
        /// </summary>
        public char Character
        {
            get
            {
                return _charicon;
            }
        }

        /// <summary>
        /// If true will disable the vanilla ui panel background.
        /// </summary>
        public bool FreeFloat
        {
            get
            {
                return _freefloat;
            }
        }

        public Color IconColor { get; set; }

        /// <summary>
        /// Whether or not to use a texture for the icon instead of a character
        /// </summary>
        public bool UseTextureIcon { get; set; }

        /// <inheritdoc/>
        public void Attatch(UIElement element,
            Vector2 distance)
        {
            AttachedElement = element;
            _distance = distance;

            if (AttachedElement != null)
            {
                var dimensions = GetDimensions();
                var elementDims = AttachedElement.GetDimensions();
                var centerDist = Vector2.Subtract(dimensions.Center(), dimensions.Position());

                Left.Set(elementDims.Center().X - centerDist.X + distance.X, 0f);
                Top.Set(elementDims.Center().Y - centerDist.Y + distance.Y, 0f);
            }
        }

        /// <inheritdoc/>
        public void Dettatch()
        {
            AttachedElement = null;
        }

        public override void Recalculate()
        {
            base.Recalculate();
            Attatch(AttachedElement, _distance);
            // Possible way to detect element recalculation is to constantly plant a seed element that sends a message when it's recalculated.
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!_freefloat)
                base.DrawSelf(spriteBatch);

            if (!UseTextureIcon)
                Utils.DrawBorderString(spriteBatch, _charicon.ToString(), GetInnerDimensions().Position(), IconColor);
        }
    }
}