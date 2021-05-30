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
        internal static IDictionary<LayerTextInfo, Func<LayerReference, string, int, bool, string>> InfoToLogicMap
            = new Dictionary<LayerTextInfo, Func<LayerReference, string, int, bool, string>>
            {
                { LayerTextInfo.Coordinates, CoordinateLogic },
                { LayerTextInfo.TextureFrameRectangle, TextureFrameBoundsLogic }
            };

        internal static string CoordinateLogic(LayerReference layer,
            string newText,
            int infoMod,
            bool interacting)
        {
            Vector2 currentCoords = layer.FrameData.Position;
            var coordList = new float[2];
            coordList[0] = currentCoords.X;
            coordList[1] = currentCoords.Y;
            
            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(coordList[infoMod]) || !interacting)
                return coordList[infoMod].ToString();

            coordList[infoMod] = float.Parse(newText);

            layer.FrameData.Position = new Vector2(coordList[0], coordList[1]);

            return newText;
        }

        internal static string TextureFrameBoundsLogic(LayerReference layer,
            string newText,
            int infoMod,
            bool interacting)
        {
            Point currentBounds = layer.Layer.Tag.Size;
            var boundList = new string[2];
            boundList[0] = currentBounds.X.ToString();
            boundList[1] = currentBounds.Y.ToString();

            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(boundList[infoMod]) || !interacting)
                return boundList[infoMod];

            if (infoMod == 1)
                currentBounds.X = int.Parse(newText);
            else
                currentBounds.Y = int.Parse(newText);

            layer.Layer.Tag = new TextureTag(layer.Layer.Tag.Name, currentBounds);
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

        internal event Func<LayerReference, string, int, bool, string> UpdateLogic;

        internal int InfoModifier { get; set; }

        internal LayerReference LayerRef { get; set; }

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
                Text = UpdateLogic(LayerRef, Text, InfoModifier, Interacting);

            base.Update(gameTime);
        }
    }
}