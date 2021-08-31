using HunterCombatMR.Builders.Animation;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Utilities;
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
        private LayerBuilder layerRef;

        public LayerText(LayerBuilder layerRef,
            LayerTextInfo infoArgs = LayerTextInfo.None)
        {
            LayerRef = layerRef;
            SetDisplayInformation(infoArgs);
            OnClick += HighlightFrame;
            OnRightClick += DeselectAllFrames;
            _displayText = "";
        }

        public LayerTextInfo DisplayInfo { get; private set; }
        public FrameIndex Keyframe { get; set; }
        public LayerBuilder LayerRef { get => layerRef; set { layerRef = value; GenerateText(); } }

        public string Text
        {
            get
            {
                return _displayText;
            }
        }

        public Color TextColor { get; set; } = Color.White;
        public Texture2D Texture { get => TextureUtils.GetTextureFromTag(new TextureTag(LayerRef.Name, Point.Zero)); }

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
            else if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(LayerRef.Name)))
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

            if (LayerRef is null)
                return;

            foreach (LayerTextInfo info in DisplayInfo.GetFlags())
            {
                _displayText = InfoText(info);
            }
        }

        private string CoordinateInfoText()
            => $"Coords: X- {LayerRef.GetLayerData(Keyframe).Position.X} Y- {LayerRef.GetLayerData(Keyframe).Position.Y} ";

        private string DepthInfoText()
            => $"Depth: {LayerRef.GetLayerData(Keyframe).Depth} ";

        private void DeselectAllFrames(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers != null && HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any())
                HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Clear();
        }

        private void HighlightFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.keyState.GetPressedKeys().Any(x => x.Equals(Keys.LeftShift)))
            {
                if (!EditorUtils.HighlightedLayerNames.Any(x => x.Equals(LayerRef.Name)))
                {
                    EditorUtils.HighlightedLayerNames.Add(LayerRef.Name);
                    return;
                }

                EditorUtils.HighlightedLayerNames.Remove(LayerRef.Name);
                return;
            }

            EditorUtils.HighlightedLayerNames.Clear();
            if (!EditorUtils.HighlightedLayerNames.Any(x => x.Equals(LayerRef.Name)))
            {
                EditorUtils.HighlightedLayerNames.Add(LayerRef.Name);
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
                    return LayerRef.Name;
            }
        }

        private string OrientationInfoText()
        {
            string text = "Orientation: ";
            SpriteEffects orientation = LayerRef.GetLayerData(Keyframe).Orientation;

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
            => $"Rotation: {MathHelper.ToDegrees(LayerRef.GetLayerData(Keyframe).Rotation)}° ";

        private string TextureBoundsInfoText()
            => $"Bounds: W- {Texture.Width} H- {LayerRef.GetLayerData(Keyframe).SheetIndex}";

        private string TextureFrameInfoText()
                    => $"Frame: {LayerRef.GetLayerData(Keyframe).SheetFrame}";

        private string TextureNameInfoText()
            => $"Texture Tag: {Texture.Name}";
    }
}