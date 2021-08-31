using HunterCombatMR.Builders.Animation;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.UI.Elements
{
    internal static class LayerInfoTextBoxLogic
    {
        internal static IDictionary<LayerTextInfo, Func<LayerBuilder, FrameIndex, string, int, bool, string>> InfoToLogicMap
            = new Dictionary<LayerTextInfo, Func<LayerBuilder, FrameIndex, string, int, bool, string>>
            {
                { LayerTextInfo.Coordinates, CoordinateLogic }
            };

        internal static string CoordinateLogic(LayerBuilder layer,
            FrameIndex keyframe,
            string newText,
            int infoMod,
            bool interacting)
        {
            var layerData = layer.GetLayerData(keyframe);
            var coordList = new int[2];
            coordList[0] = layerData.Position.X;
            coordList[1] = layerData.Position.Y;

            if (string.IsNullOrWhiteSpace(newText) || newText.Equals(coordList[infoMod]) || !interacting)
                return coordList[infoMod].ToString();

            coordList[infoMod] = int.Parse(newText);
            layerData.Position = new Point(coordList[0], coordList[1]);

            layer.SetLayerData(layerData);

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

        internal event Func<LayerBuilder, FrameIndex, string, int, bool, string> UpdateLogic;

        internal int InfoModifier { get; set; }

        internal FrameIndex Keyframe { get; set; }
        internal LayerBuilder LayerRef { get; set; }

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
            Text = UpdateLogic(LayerRef, Keyframe, Text, InfoModifier, Interacting);

            base.Update(gameTime);
        }
    }
}