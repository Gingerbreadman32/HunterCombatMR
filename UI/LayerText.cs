using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    internal class LayerText
        : UIElement
    {
        #region Private Fields

        private string _displayText;

        #endregion Private Fields

        #region Public Constructors

        public LayerText(IAnimation animation,
            string layerName,
            int currentKeyFrame,
            LayerTextInfo infoArgs = LayerTextInfo.None)
        {
            CurrentKeyFrame = currentKeyFrame;
            SetLayer(animation, layerName);
            SetDisplayInformation(infoArgs);
            OnClick += HighlightFrame;
            OnRightClick += DeselectAllFrames;
        }

        public LayerText(AnimationLayer layer,
            int currentKeyFrame,
            LayerTextInfo infoArgs = LayerTextInfo.None)
        {
            CurrentKeyFrame = currentKeyFrame;
            SetLayer(layer);
            SetDisplayInformation(infoArgs);
        }

        #endregion Public Constructors

        #region Public Properties

        public IAnimation AnimationReference { get; protected set; }
        public int CurrentKeyFrame { get; }
        public LayerTextInfo DisplayInfo { get; protected set; }
        public AnimationLayer Layer { get; protected set; }

        public string Text
        {
            get
            {
                return _displayText;
            }
        }

        public Color TextColor { get; set; } = Color.White;

        #endregion Public Properties

        #region Public Methods

        public override void Recalculate()
        {
            base.Recalculate();
            Vector2 textSize = new Vector2(Main.fontMouseText.MeasureString(_displayText.ToString()).X, 16f);
            MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
            MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
        }

        public void SetDisplayInformation(LayerTextInfo infoArgs)
        {
            DisplayInfo = infoArgs;
            GenerateText();
        }

        public void SetLayer(IAnimation animation,
            string layerName)
        {
            AnimationReference = animation;
            Layer = animation.GetLayer(layerName);
            GenerateText();
        }

        public void SetLayer(AnimationLayer layer)
        {
            Layer = layer;
            GenerateText();
        }

        public override void Update(GameTime gameTime)
        {
            GenerateText();
            base.Update(gameTime);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Color displayColor = TextColor;
            
            var MousePosition = new Vector2(Main.mouseX, Main.mouseY);
            if (Parent.GetElementAt(MousePosition) == this)
                displayColor = Color.LightBlue;
            else if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(Layer.Name)))
                displayColor = Color.Red;

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();
            Vector2 textSize = new Vector2(Main.fontMouseText.MeasureString(_displayText.ToString()).X, 16f);
            pos.Y -= 2f;
            pos.X += (innerDimensions.Width - textSize.X) * 0.5f;

            Utils.DrawBorderString(spriteBatch, Text, pos, displayColor, 1f);
        }

        protected void GenerateText()
        {
            if (Layer != null && (Layer.GetActiveAtKeyFrame(CurrentKeyFrame) || DisplayInfo == LayerTextInfo.None))
            {
                foreach (LayerTextInfo info in DisplayInfo.GetFlags())
                {
                    _displayText = InfoText(info);
                }
            }
            else
            {
                _displayText = "";
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private string CoordinateInfoText()
            => $"Coords: X- {Layer.KeyFrames[CurrentKeyFrame].Position.X} Y- {Layer.KeyFrames[CurrentKeyFrame].Position.Y} ";

        private string DepthInfoText()
            => $"Default Depth: {Layer.GetDepthAtKeyFrame(CurrentKeyFrame)} ";

        private void DeselectAllFrames(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers != null && HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any())
                HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Clear();
        }

        private void HighlightFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.keyState.GetPressedKeys().Any(x => x.Equals(Keys.LeftShift)))
            {
                if (!HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(Layer.Name)))
                {
                    HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Add(Layer.Name);
                }
                else
                {
                    HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Remove(Layer.Name);
                }
            }
            else
            {
                HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Clear();
                if (!HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(Layer.Name)))
                {
                    HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Add(Layer.Name);
                }
            }
        }

        private string OrientationInfoText()
        {
            string text = "Orientation: ";
            SpriteEffects orientation = Layer.GetOrientationAtKeyFrame(CurrentKeyFrame);

            switch (orientation)
            {
                case SpriteEffects.None:
                    text += "None ";
                    break;

                case SpriteEffects.FlipHorizontally:
                    text += "Horizontal ";
                    break;

                case SpriteEffects.FlipVertically:
                    text += "Vertical ";
                    break;

                case SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically:
                    text += "Both ";
                    break;
            }
            return text;
        }

        private string InfoText(LayerTextInfo flag)
        {
            switch (flag)
            {
                case LayerTextInfo.Coordinates:
                    return CoordinateInfoText();

                case LayerTextInfo.DefaultDepth:
                    return DepthInfoText();

                case LayerTextInfo.Rotation:
                    return RotationInfoText();

                case LayerTextInfo.Orientation:
                    return OrientationInfoText();

                case LayerTextInfo.TextureFrames:
                    return TextureFrameInfoText();

                case LayerTextInfo.TextureName:
                    return TextureNameInfoText();

                case LayerTextInfo.TextureFrameRectangle:
                    return TextureBoundsInfoText();

                default:
                    return Layer.Name;
            }
        }

        private string RotationInfoText()
            => $"Rotation: {MathHelper.ToDegrees(Layer.GetRotationAtKeyFrame(CurrentKeyFrame))}° ";

        private string TextureBoundsInfoText()
            => $"Bounds: W- {Layer.SpriteFrameRectangle.Width} H- {Layer.SpriteFrameRectangle.Height}";

        private string TextureFrameInfoText()
                    => $"Frame: {Layer.GetTextureFrameAtKeyFrame(CurrentKeyFrame)} / {Layer.GetSpriteTextureFrameTotal()} ";

        private string TextureNameInfoText()
            => $"Texture: {Layer.Texture.Name.Split('/')[Layer.Texture.Name.Split('/').Length - 1]}";

        #endregion Private Methods
    }
}