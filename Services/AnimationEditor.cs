using HunterCombatMR.Builders.Animation;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
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

        public AnimationBuilder CurrentAnimationEditing { get; set; }

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

        public void AdjustPositionLogic(IEnumerable<LayerBuilder> layers,
            int direction = 1)
        {
            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            var nudgeAmount = NudgeLogic();

            foreach (var layer in layers)
            {
                if (layer.KeyframeEditing.Orientation.Equals(SpriteEffects.FlipHorizontally))
                    direction *= -1;

                var layerNudgeAmount = nudgeAmount;
                layerNudgeAmount.X *= direction;
                layer.KeyframeEditing.Position = new Point(layerNudgeAmount.X + layer.KeyframeEditing.Position.X, 
                    layerNudgeAmount.Y + layer.KeyframeEditing.Position.Y);
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

        private Point NudgeLogic()
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

            return highlightedNudge;
        }
    }
}