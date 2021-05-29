using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.UI.Elements
{
    internal static class LayerInfoTextBoxLogic
    {
        internal static IDictionary<LayerTextInfo, Func<Layer, int, string, int, bool, string>> InfoToLogicMap
            = new Dictionary<LayerTextInfo, Func<Layer, int, string, int, bool, string>>
            {
                { LayerTextInfo.Coordinates, CoordinateLogic },
                { LayerTextInfo.TextureFrameRectangle, TextureFrameBoundsLogic }
            };

        internal static string CoordinateLogic(Layer layer,
            int keyframe,
            string newText,
            int infoMod,
            bool interacting)
        {
            var layerData = layer.KeyFrameData[keyframe].Save();
            Vector2 currentCoords = layer.KeyFrameData[keyframe].Position;
            var coordList = new string[2];
            coordList[0] = currentCoords.X.ToString();
            coordList[1] = currentCoords.Y.ToString();

            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(coordList[infoMod]))
                return coordList[infoMod];

            if (interacting)
            {
                if (infoMod == 1)
                    layerData[LayerDataParameters.PositionY] = float.Parse(newText);
                else
                    layerData[LayerDataParameters.PositionX] = float.Parse(newText);

                layer.KeyFrameData[keyframe] = new LayerData(layerData);
            }
            else
            {
                newText = coordList[infoMod];
            }

            return newText;
        }

        internal static string TextureFrameBoundsLogic(Layer layer,
            int keyframe,
            string newText,
            int infoMod,
            bool interacting)
        {
            Point currentBounds = layer.Tag.Size;
            var boundList = new string[2];
            boundList[0] = currentBounds.X.ToString();
            boundList[1] = currentBounds.Y.ToString();

            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(boundList[infoMod]))
                return boundList[infoMod];

            if (infoMod == 1)
                currentBounds.X = int.Parse(newText);
            else
                currentBounds.Y = int.Parse(newText);

            layer.Tag = new TextureTag(layer.Tag.DisplayName, currentBounds);
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

        internal event Func<Layer, int, string, int, bool, string> UpdateLogic;

        internal int InfoModifier { get; set; }

        internal int KeyFrame { get; set; }

        internal Layer Layer { get; set; }

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
            if (Layer?.IsActive(KeyFrame) ?? false)
                Text = UpdateLogic(Layer, KeyFrame, Text, InfoModifier, Interacting);
            else
                Text = "";

            base.Update(gameTime);
        }

        internal void SetLayerAndKeyFrame(Layer layer,
                    int keyFrame)
        {
            if (layer?.IsActive(KeyFrame) ?? false)
            {
                Layer = layer;
                KeyFrame = keyFrame;
            }
        }
    }
}