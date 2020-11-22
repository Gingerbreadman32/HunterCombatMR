using HunterCombatMR;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace AnimationEngine.Services
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
            if (!_onionSkin || !CurrentEditMode.Equals(EditorMode.EditMode))
                return false;

            color.A = 30;

            foreach (var layer in layerData.Layers.Where(f => f.Frames.ContainsKey(keyFrameToDraw)).OrderByDescending(x => x.Frames[keyFrameToDraw].LayerDepth))
            {
                HunterCombatPlayer.CombatLimbDraw(drawInfo, 
                    HunterCombatPlayer.CreateTextureString(layer.Name), 
                    layer.GetCurrentFrameRectangle(keyFrameToDraw), 
                    layer.Frames[keyFrameToDraw],
                    color)
                    .Draw(Main.spriteBatch);
            }

            return true;
        }

        public void AdjustPositionLogic(ActionAnimation animation,
            int direction = 1)
        {
            List<string> framelessNames = new List<string>(HighlightedLayers);
            framelessNames.ForEach(layerName => layerName = layerName.Split('-')[0].Trim());
            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);
            int currentFrame = animation.AnimationData.GetCurrentKeyFrameIndex();
            foreach (string layerName in framelessNames)
            {
                AnimationLayer layer = animation.LayerData.Layers.FirstOrDefault(x => x.Name.Equals(layerName));

                if (layer == null)
                    continue;

                if (layer.Frames[currentFrame].SpriteOrientation.Equals(SpriteEffects.FlipHorizontally))
                    direction *= -1;

                layer.SetPositionAtFrame(currentFrame, NudgeLogic(layer.Frames[currentFrame].Position, direction));
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

        private Vector2 NudgeLogic(Vector2 initialPosition,
            int direction)
        {
            var highlightedNudge = new Point();

            if (_nudgeCooldown > 0 && _nudgeCooldown < _nudgeCooldownMax)
            {
                _nudgeCooldown--;
            }
            else if (_nudgeCooldown == _nudgeCooldownMax)
            {
                _nudgeCooldown--;
                if (PlayerInput.Triggers.JustReleased.Down)
                {
                    var nudgeAmount = (initialPosition.Y % 2 == 0 || initialPosition.Y == 0) ? 2 : 1;
                    var newNudge = new Point(0, 1);
                    highlightedNudge = newNudge;
                }
                else if (PlayerInput.Triggers.JustReleased.Up)
                {
                    var nudgeAmount = (initialPosition.Y % 2 == 0 || initialPosition.Y == 0) ? 2 : 1;
                    var newNudge = new Point(0, -1);
                    highlightedNudge = newNudge;
                }
                else if (PlayerInput.Triggers.JustReleased.Left)
                {
                    var nudgeAmount = (initialPosition.X % 2 == 0 || initialPosition.X == 0) ? 2 : 1;
                    var newNudge = new Point(-1 * direction, 0);
                    highlightedNudge = newNudge;
                }
                else if (PlayerInput.Triggers.JustReleased.Right)
                {
                    var nudgeAmount = (initialPosition.X % 2 == 0 || initialPosition.X == 0) ? 2 : 1;
                    var newNudge = new Point(1 * direction, 0);
                    highlightedNudge = newNudge;
                } else
                {
                    _nudgeCooldown++;
                }
                
            }
            else
            {
                _nudgeCooldown = _nudgeCooldownMax;
            }

            return initialPosition + highlightedNudge.ToVector2();
        }
    }
}