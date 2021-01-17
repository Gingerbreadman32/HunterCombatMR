using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.UI
{
    internal static class LayerInfoTextBoxLogic
    {
        #region Internal Fields

        internal static IDictionary<LayerTextInfo, Func<AnimationLayer, int, string, int, string>> InfoToLogicMap
            = new Dictionary<LayerTextInfo, Func<AnimationLayer, int, string, int, string>>
            {
                { LayerTextInfo.Coordinates, CoordinateLogic }
            };

        #endregion Internal Fields

        #region Internal Methods

        internal static string CoordinateLogic(AnimationLayer layer,
            int keyframe,
            string newText,
            int infoMod)
        {
            Vector2 currentCoords = layer.GetPositionAtKeyFrame(keyframe);

            if (string.IsNullOrWhiteSpace(newText))
                return (infoMod == 1) ? currentCoords.Y.ToString() : currentCoords.X.ToString();

            if (Main.keyState.GetPressedKeys().Any(x => x.Equals(Keys.Subtract)))
            {
                if (newText.Contains('-'))
                    newText = newText.Remove(0);
                else
                    newText = newText.Insert(0, "-");
            }

            if (infoMod == 1)
            {
                if (currentCoords.Y.ToString().Equals(newText))
                    return currentCoords.Y.ToString();
                else
                {
                    layer.SetPositionAtKeyFrame(keyframe, new Vector2(currentCoords.X, float.Parse(newText)));
                    return newText;
                }
            }
            else
            {
                if (currentCoords.X.ToString().Equals(newText))
                    return currentCoords.X.ToString();
                else
                {
                    layer.SetPositionAtKeyFrame(keyframe, new Vector2(float.Parse(newText), currentCoords.Y));
                    return newText;
                }
            }
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

        internal event Func<AnimationLayer, int, string, int, string> UpdateLogic;

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
            if (Layer != null)
                Text = UpdateLogic(Layer, KeyFrame, Text, InfoModifier);

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