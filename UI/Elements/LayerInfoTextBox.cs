using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.UI.Elements
{
    internal static class LayerInfoTextBoxLogic
    {
        internal static IDictionary<LayerTextInfo, Func<AnimationLayer, int, string, int, bool, string>> InfoToLogicMap
            = new Dictionary<LayerTextInfo, Func<AnimationLayer, int, string, int, bool, string>>
            {
                { LayerTextInfo.Coordinates, CoordinateLogic },
                { LayerTextInfo.TextureFrameRectangle, TextureFrameBoundsLogic }
            };

        internal static string CoordinateLogic(AnimationLayer layer,
            int keyframe,
            string newText,
            int infoMod,
            bool interacting)
        {
            Vector2 currentCoords = layer.GetPosition(keyframe);
            var coordList = new string[2];
            coordList[0] = currentCoords.X.ToString();
            coordList[1] = currentCoords.Y.ToString();

            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(coordList[infoMod]))
                return coordList[infoMod];

            if (interacting)
            {
                if (infoMod == 1)
                    layer.SetPosition(keyframe, new Vector2(currentCoords.X, float.Parse(newText)));
                else
                    layer.SetPosition(keyframe, new Vector2(float.Parse(newText), currentCoords.Y));
            }
            else
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
    }

    internal class LayerInfoTextBox
            : NumberInputBox
    {
        private LayerTextInfo _infoType;

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

        internal event Func<AnimationLayer, int, string, int, bool, string> UpdateLogic;

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

        public override void Update(GameTime gameTime)
        {
            if (Layer != null && Layer.IsActive(KeyFrame))
                Text = UpdateLogic(Layer, KeyFrame, Text, InfoModifier, Interacting);
            else
                Text = "";

            base.Update(gameTime);
        }

        internal void SetLayerAndKeyFrame(AnimationLayer layer,
                    int keyFrame)
        {
            if (layer != null && layer.IsActive(keyFrame))
            {
                Layer = layer;
                KeyFrame = keyFrame;
            }
        }
    }
}