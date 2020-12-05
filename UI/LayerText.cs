using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
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

        public LayerText(AnimationEngine.Models.Animation animation,
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

        #endregion Public Constructors

        #region Public Properties

        public LayerTextInfo DisplayInfo { get; protected set; }
        public AnimationEngine.Models.Animation AnimationReference { get; protected set; }
        public AnimationLayer Layer { get; protected set; }
        public int CurrentKeyFrame { get; }
        

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
            GenerateText();
            base.Recalculate();
        }

        public void SetDisplayInformation(LayerTextInfo infoArgs)
        {
            DisplayInfo = infoArgs;
            GenerateText();
        }

        public void SetLayer(AnimationEngine.Models.Animation animation,
            string layerName)
        {
            AnimationReference = animation;
            Layer = animation.GetLayer(layerName);
            GenerateText();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Color displayColor = TextColor;

            if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(Layer.Name)))
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
            _displayText = Layer.Name;

            if (DisplayInfo > 0)
            {
                _displayText += " - ";

                if (DisplayInfo.HasFlag(LayerTextInfo.Coordinates))
                    _displayText += CoordinateInfoText(CurrentKeyFrame, Layer);

                if (DisplayInfo.HasFlag(LayerTextInfo.Depth))
                    _displayText += DepthInfoText(CurrentKeyFrame, Layer);
            }

            Vector2 textSize = new Vector2(Main.fontMouseText.MeasureString(_displayText.ToString()).X, 16f);
            MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
            MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
        }

        #endregion Protected Methods

        #region Private Methods

        private string CoordinateInfoText(int currentFrame,
            AnimationLayer layer)
            => $"X: {layer.KeyFrames[currentFrame].Position.X} Y: {layer.KeyFrames[currentFrame].Position.Y} ";

        private string DepthInfoText(int currentFrame,
            AnimationLayer layer)
            => $"D: {layer.GetDepthAtKeyFrame(currentFrame)} ";

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

        #endregion Private Methods
    }
}