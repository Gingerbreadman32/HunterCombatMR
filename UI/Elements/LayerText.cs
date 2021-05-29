using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace HunterCombatMR.UI.Elements
{
    internal class LayerText
        : UIElement
    {
        private string _displayText;

        public LayerText(ICustomAnimationV2 animation,
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

        public LayerText(Layer layer,
            int currentKeyFrame,
            LayerTextInfo infoArgs = LayerTextInfo.None)
        {
            CurrentKeyFrame = currentKeyFrame;
            SetLayer(layer);
            SetDisplayInformation(infoArgs);
        }

        public ICustomAnimationV2 AnimationReference { get; protected set; }
        public FrameIndex CurrentKeyFrame { get; }
        public LayerTextInfo DisplayInfo { get; protected set; }
        public Layer Layer { get; protected set; }

        public string Text
        {
            get
            {
                return _displayText;
            }
        }

        public Color TextColor { get; set; } = Color.White;

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

        public void SetLayer(ICustomAnimationV2 animation,
            string layerName)
        {
            AnimationReference = animation;
            Layer = animation.Layers[layerName];
            GenerateText();
        }

        public void SetLayer(Layer layer)
        {
            Layer = layer;
            GenerateText();
        }

        public override void Update(GameTime gameTime)
        {
            GenerateText();
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Color displayColor = TextColor;

            var MousePosition = new Vector2(Main.mouseX, Main.mouseY);
            if (Parent.GetElementAt(MousePosition) == this)
                displayColor = Color.LightBlue;
            else if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(Layer.DisplayName)))
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
            _displayText = "";

            if ((Layer.IsActive(CurrentKeyFrame) || DisplayInfo == LayerTextInfo.None))
            {
                foreach (LayerTextInfo info in DisplayInfo.GetFlags())
                {
                    _displayText = InfoText(info);
                }
            }
        }

        private string CoordinateInfoText()
            => $"Coords: X- {Layer.KeyFrameData[CurrentKeyFrame].Position.X} Y- {Layer.KeyFrameData[CurrentKeyFrame].Position.Y} ";

        private string DepthInfoText()
            => $"Depth: {Layer.GetCurrentLayerDepth(CurrentKeyFrame)} ";

        private void DeselectAllFrames(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers != null && HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any())
                HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Clear();
        }

        private void HighlightFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.keyState.GetPressedKeys().Any(x => x.Equals(Keys.LeftShift)))
            {
                if (!HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(Layer.DisplayName)))
                {
                    HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Add(Layer.DisplayName);
                }
                else
                {
                    HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Remove(Layer.DisplayName);
                }
            }
            else
            {
                HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Clear();
                if (!HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(Layer.DisplayName)))
                {
                    HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Add(Layer.DisplayName);
                }
            }
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
                    return Layer.DisplayName;
            }
        }

        private string OrientationInfoText()
        {
            string text = "Orientation: ";
            SpriteEffects orientation = Layer.KeyFrameData[CurrentKeyFrame].Orientation;

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

        private string RotationInfoText()
            => $"Rotation: {MathHelper.ToDegrees(Layer.KeyFrameData[CurrentKeyFrame].Rotation)}° ";

        private string TextureBoundsInfoText()
            => $"Bounds: W- {Layer.Tag.Size.X} H- {Layer.Tag.Size.Y}";

        private string TextureFrameInfoText()
                    => $"Frame: {Layer.KeyFrameData[CurrentKeyFrame].SheetFrame}";

        private string TextureNameInfoText()
            => $"Texture Tag: {Layer.Tag.DisplayName}";
    }
}