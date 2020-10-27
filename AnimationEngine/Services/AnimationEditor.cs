using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace AnimationEngine.Services
{
    public class AnimationEditor
    {
        public List<string> HighlightedLayers { get; set; }

        public Point? SelectedLayerNudgeAmount { get; set; }

        public int? NudgeCooldown { get; set; }

        public string SelectedLayer { get; set; }

        public EditorMode CurrentEditMode { get; set; }

        public AnimationEditor()
        {
            HighlightedLayers = new List<string>();
            SelectedLayer = "";
            SelectedLayerNudgeAmount = new Point(0, 0);
            NudgeCooldown = 2;
            CurrentEditMode = EditorMode.None;
        }

        public void Dispose()
        {
            NudgeCooldown = null;
            SelectedLayerNudgeAmount = null;
            HighlightedLayers = null;
            SelectedLayer = null;
        }

        public DrawData AdjustPositionLogic(DrawData drawData,
            IDictionary<string, Vector2> positions,
            string layerName)
        {
            var framelessName = layerName.Split('-')[0].Trim();
            var value = drawData;
            var MousePosition = new Vector2(Main.mouseX, Main.mouseY);
            var texPosition = new Rectangle((int)positions[layerName].X, (int)positions[layerName].Y, value.sourceRect.GetValueOrDefault().Width, value.sourceRect.GetValueOrDefault().Height);
            if (HighlightedLayers.Contains(framelessName))
            {
                value.color = Color.Red;

                positions[layerName] += NudgeLogic(positions[layerName]).ToVector2();

                if (PlayerInput.MouseInfoOld.LeftButton.Equals(ButtonState.Pressed) && Main.mouseLeft && SelectedLayer == "")
                {
                    //SelectedLayer = framelessName;
                }
            }

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

            return value;
        }

        private Point NudgeLogic(Vector2 initialPosition)
        {
            var highlightedNudge = new Point();
            if (SelectedLayerNudgeAmount.HasValue && NudgeCooldown.HasValue)
            {
                if (NudgeCooldown.Value > 0 && NudgeCooldown < 2)
                {
                    NudgeCooldown = NudgeCooldown.Value - 1;
                }
                else if (NudgeCooldown == 2)
                {
                    if (PlayerInput.Triggers.JustReleased.Down)
                    {
                        var nudgeAmount = (initialPosition.Y % 2 == 0 || initialPosition.Y == 0) ? 2 : 1;
                        var newNudge = new Point(SelectedLayerNudgeAmount.Value.X, SelectedLayerNudgeAmount.Value.Y + nudgeAmount);
                        highlightedNudge = newNudge;
                        NudgeCooldown = NudgeCooldown.Value - 1;
                    }
                    else if (PlayerInput.Triggers.JustReleased.Up)
                    {
                        var nudgeAmount = (initialPosition.Y % 2 == 0 || initialPosition.Y == 0) ? 2 : 1;
                        var newNudge = new Point(SelectedLayerNudgeAmount.Value.X, SelectedLayerNudgeAmount.Value.Y - nudgeAmount);
                        highlightedNudge = newNudge;
                        NudgeCooldown = NudgeCooldown.Value - 1;
                    }
                    else if (PlayerInput.Triggers.JustReleased.Left)
                    {
                        var nudgeAmount = (initialPosition.X % 2 == 0 || initialPosition.X == 0) ? 2 : 1;
                        var newNudge = new Point(SelectedLayerNudgeAmount.Value.X - nudgeAmount, SelectedLayerNudgeAmount.Value.Y);
                        highlightedNudge = newNudge;
                        NudgeCooldown = NudgeCooldown.Value - 1;
                    }
                    else if (PlayerInput.Triggers.JustReleased.Right)
                    {
                        var nudgeAmount = (initialPosition.X % 2 == 0 || initialPosition.X == 0) ? 2 : 1;
                        var newNudge = new Point(SelectedLayerNudgeAmount.Value.X + nudgeAmount, SelectedLayerNudgeAmount.Value.Y);
                        highlightedNudge = newNudge;
                        NudgeCooldown = NudgeCooldown.Value - 1;
                    }
                }
                else
                {
                    NudgeCooldown = 2;
                }
            }

            return highlightedNudge;
        }
    }
}