using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.UI
{
    internal static class LayerInfoTextBoxLogic
    {
        #region Internal Fields

        internal static IDictionary<LayerTextInfo, Func<AnimationLayer, int, string, int, bool, string>> InfoToLogicMap
            = new Dictionary<LayerTextInfo, Func<AnimationLayer, int, string, int, bool, string>>
            {
                { LayerTextInfo.Coordinates, CoordinateLogic },
                { LayerTextInfo.TextureFrameRectangle, TextureFrameBoundsLogic }
            };

        #endregion Internal Fields

        #region Internal Methods

        internal static string CoordinateLogic(AnimationLayer layer,
            int keyframe,
            string newText,
            int infoMod,
            bool interacting)
        {
            Vector2 currentCoords = layer.GetPositionAtKeyFrame(keyframe);
            var coordList = new string[2];
            coordList[0] = currentCoords.X.ToString();
            coordList[1] = currentCoords.Y.ToString();

            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(coordList[infoMod]))
                return coordList[infoMod];

            if (interacting)
            {
                if (infoMod == 1)
                    layer.SetPositionAtKeyFrame(keyframe, new Vector2(currentCoords.X, float.Parse(newText)));
                else
                    layer.SetPositionAtKeyFrame(keyframe, new Vector2(float.Parse(newText), currentCoords.Y));
            } else
            {
                newText = coordList[infoMod];
            }

            return newText;
        }

        internal static string TextureFrameBoundsLogic(AnimationLayer layer,
            int keyframe,
            string newText,
            int infoMod,
            bool interacting)
        {
            Rectangle currentBounds = layer.SpriteFrameRectangle;
            var boundList = new string[2];
            boundList[0] = currentBounds.Width.ToString();
            boundList[1] = currentBounds.Height.ToString();

            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(boundList[infoMod]))
                return boundList[infoMod];

            if (infoMod == 1)
                currentBounds.Height = int.Parse(newText);
            else
                currentBounds.Width = int.Parse(newText);

            layer.SpriteFrameRectangle = currentBounds;
            return newText;
        }

        #endregion Internal Methods

    }

    internal class LayerInfoTextBox
            : NumberInputBox
    {
        #region Private Fields

        private LayerTextInfo _infoType;

        #endregion Private Fields

        #region Internal Constructors

        internal LayerInfoTextBox(string hintText,
            LayerTextInfo infoType,
            int maxLength = 0,
            bool isLarge = false,
            bool startHidden = false,
            string defaultText = null,
            int infoMod = 0)
            : base(hintText, maxLength, isLarge, startHidden, defaultText)
        {
            TextInfoType = infoType;
            InfoModifier = infoMod;
        }

        #endregion Internal Constructors

        #region Internal Events

        internal event Func<AnimationLayer, int, string, int, bool, string> UpdateLogic;

        #endregion Internal Events

        #region Internal Properties

        internal int InfoModifier { get; set; }

        internal int KeyFrame { get; set; }

        internal AnimationLayer Layer { get; set; }

        internal LayerTextInfo TextInfoType
        {
            get { return _infoType; }

            set
            {
                _infoType = value;
                if (LayerInfoTextBoxLogic.InfoToLogicMap.ContainsKey(value))
                    UpdateLogic += LayerInfoTextBoxLogic.InfoToLogicMap[value];
                else
                    throw new Exception("Layer Text Info type missing from logic database!");
            }
        }

        #endregion Internal Properties

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (Layer != null && Layer.GetActiveAtKeyFrame(KeyFrame))
                Text = UpdateLogic(Layer, KeyFrame, Text, InfoModifier, Interacting);
            else
                Text = "";

            base.Update(gameTime);
        }

        #endregion Public Methods

        #region Internal Methods

        internal void SetLayerAndKeyFrame(AnimationLayer layer,
                    int keyFrame)
        {
            if (layer != null && layer.GetActiveAtKeyFrame(keyFrame))
            {
                Layer = layer;
                KeyFrame = keyFrame;
            }
        }

        #endregion Internal Methods
    }
}