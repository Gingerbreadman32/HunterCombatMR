﻿using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace HunterCombatMR.Services
{
    public class AnimationEditor
    {
        private const int _nudgeCooldownMax = 2;

        private int _nudgeCooldown = _nudgeCooldownMax;

        private bool _onionSkin;

        public bool OnionSkin { 
            get 
            { 
                return _onionSkin; 
            } 
        }

        public List<string> HighlightedLayers { get; set; }

        public EditorMode CurrentEditMode { get; set; }

        public PlayerAnimation CurrentAnimationEditing { get; set; }

        public bool AnimationEdited { get; set; }

        public AnimationEditor()
        {
            HighlightedLayers = new List<string>();
            CurrentEditMode = EditorMode.None;
            _onionSkin = false;
        }

        public void Dispose()
        {
            HighlightedLayers = null;
        }

        public void CloseEditor()
        {
            CurrentEditMode = EditorMode.None;
            HighlightedLayers.Clear();
        }

        internal void ToggleOnionSkin()
        {
            _onionSkin ^= true;
        }

        public bool DrawOnionSkin(PlayerDrawInfo drawInfo,
            LayerData layerData,
            int keyFrameToDraw,
            Color color)
        {
            if (!_onionSkin)
                return false;

            color.A = 30;

            foreach (var layer in layerData.Layers.Where(f => f.KeyFrames.ContainsKey(keyFrameToDraw) && f.IsActive(keyFrameToDraw)).OrderByDescending(x => x.KeyFrames[keyFrameToDraw].LayerDepth))
            {
                PlayerAnimation.CombatLimbDraw(drawInfo,
                    layer.Texture, 
                    layer.GetFrameRectangle(keyFrameToDraw), 
                    layer.KeyFrames[keyFrameToDraw],
                    color)
                    .Draw(Main.spriteBatch);
            }

            return true;
        }

        public void AdjustPositionLogic(PlayerAnimation animation,
            int direction = 1)
        {
            List<string> framelessNames = new List<string>(HighlightedLayers);
            framelessNames.ForEach(layerName => layerName = layerName.Split('-')[0].Trim());
            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);
            int currentFrame = animation.AnimationData.CurrentKeyFrameIndex;

            var nudgeAmount = NudgeLogic();

            foreach (string layerName in framelessNames)
            {
                AnimationLayer layer = animation.LayerData.Layers.FirstOrDefault(x => x.Name.Equals(layerName));

                if (layer == null)
                    continue;

                if (layer.KeyFrames[currentFrame].SpriteOrientation.Equals(SpriteEffects.FlipHorizontally))
                    direction *= -1;

                var layerNudgeAmount = nudgeAmount;
                layerNudgeAmount.X *= direction;

                layer.SetPosition(currentFrame, layer.KeyFrames[currentFrame].Position + layerNudgeAmount);
            }
            /*
            if (SelectedLayer == layerName)
            {
                value.color = Color.Green;
                var dragPosition = new Vector2(MousePosition.X - (value.sourceRect.GetValueOrDefault().Width / 2), MousePosition.Y - (value.sourceRect.GetValueOrDefault().Height / 2));

                positions[layerName] = dragPosition + SelectedLayerNudgeAmount.GetValueOrDefault().ToVector2();
            }

            if (Main.mouseLeftRelease)
            {
                SelectedLayer = "";
                SelectedLayerNudgeAmount = new Point(0, 0);
            }
            */
        }

        private Vector2 NudgeLogic()
        {
            var highlightedNudge = new Point();

            if (_nudgeCooldown > 0 && _nudgeCooldown < _nudgeCooldownMax)
            {
                _nudgeCooldown--;
            }
            else if (_nudgeCooldown == _nudgeCooldownMax)
            {
                _nudgeCooldown--;
                Point newNudge = new Point(0, 0);
                int nudgeamount = (Main.keyState.GetPressedKeys().Any(x => x.Equals(Keys.LeftShift))) ? 5 : 1;
                if (PlayerInput.Triggers.JustReleased.Down)
                {
                    newNudge.Y = nudgeamount;
                }
                else if (PlayerInput.Triggers.JustReleased.Up)
                {
                    newNudge.Y = -nudgeamount;
                }
                else if (PlayerInput.Triggers.JustReleased.Left)
                {
                    newNudge.X = -nudgeamount;
                }
                else if (PlayerInput.Triggers.JustReleased.Right)
                {
                    newNudge.X = nudgeamount;
                } else
                {
                    _nudgeCooldown++;
                }

                highlightedNudge = newNudge;
            }
            else
            {
                _nudgeCooldown = _nudgeCooldownMax;
            }

            return highlightedNudge.ToVector2();
        }
    }
}