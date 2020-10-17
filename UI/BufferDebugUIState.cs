using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    public class BufferDebugUIState
        : UIState
    {
        private HunterCombatPlayer _currentPlayer;

        private UIPanel _bufferpanel;

        private UIPanel _layerpanel;

        public override void OnInitialize()
        {
            _bufferpanel = new UIPanel();
            _bufferpanel.Width.Set(300, 0);
            _bufferpanel.Height.Set(100, 0);
            _bufferpanel.BackgroundColor = Microsoft.Xna.Framework.Color.Red;
            _bufferpanel.BackgroundColor.A = 50;
            _bufferpanel.HAlign = 0.5f;
            _bufferpanel.VAlign = 0.95f;
            Append(_bufferpanel);

            _layerpanel = new UIPanel();
            _layerpanel.Width.Set(550, 0);
            _layerpanel.Height.Set(150, 0);
            _layerpanel.BackgroundColor = Microsoft.Xna.Framework.Color.Blue;
            _layerpanel.BackgroundColor.A = 50;
            _layerpanel.HAlign = 0.5f;
            _layerpanel.VAlign = 0f;
            Append(_layerpanel);

            var buttonEA = new UIAutoScaleTextTextPanel<string>(HunterCombatMR.EditMode.GetDescription())
            {
                TextColor = Color.White,
                Width = new StyleDimension(50f, 0),
                Height =
                {
                    Pixels = 40f
                },
                VAlign = 1f,
                Top =
                {
                    Pixels = -65f
                }
            }.WithFadedMouseOver();
            buttonEA.OnClick += OnButtonClick;

            Append(buttonEA);
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (HunterCombatMR.EditMode.Equals(EditorMode.EditMode))
            {
                HunterCombatMR.EditMode = EditorMode.None;
                (listeningElement as UIAutoScaleTextTextPanel<string>).TextColor = Color.White;
            } else if (HunterCombatMR.EditMode.Equals(EditorMode.ViewMode))
            {
                HunterCombatMR.EditMode = EditorMode.EditMode;
                (listeningElement as UIAutoScaleTextTextPanel<string>).TextColor = Color.Green;
            } else
            {
                HunterCombatMR.EditMode = EditorMode.ViewMode;
                (listeningElement as UIAutoScaleTextTextPanel<string>).TextColor = Color.Orange;
            }

            (listeningElement as UIAutoScaleTextTextPanel<string>).SetText(HunterCombatMR.EditMode.GetDescription());
        }

        public override void Update(GameTime gameTime)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            var buffers = new List<string>();
            foreach (var buffer in _currentPlayer.InputBufferInfo.BufferedComboInputs)
            {
                buffers.Add($"{buffer.Input.ToString()} - {buffer.FramesSinceBuffered}");
            }

            var activelayers = new List<string>();
            var layers = PlayerHooks.GetDrawLayers(_currentPlayer.player);
            /*
            foreach (var layer in layers.Where(x => x.visible))
            {
                var parent = layer.parent;
                var parenttext = "";
                if (parent != null)
                    parenttext = $" - parent.Name";

                activelayers.Add($"{layer.Name}{parent}");
            }*/

            foreach (var layer in _currentPlayer.LayerPositions)
            {
                activelayers.Add($"{layer.Key} - X: {layer.Value.X - (int)_currentPlayer.player.position.X + (int)Main.screenPosition.X} Y: {layer.Value.Y - (int)_currentPlayer.player.position.Y + (int)Main.screenPosition.Y}");
            }

            ListVariables(_bufferpanel, buffers);
            ListVariables(_layerpanel, activelayers);
        }

        private void ListVariables(UIPanel panel,
            IEnumerable<string> variables)
        {
            panel.RemoveAllChildren();
            var topPadding = 0;
            var leftPadding = 0;
            foreach (var text in variables)
            {
                UIText vartext = new UIText(text);
                vartext.PaddingTop = topPadding;
                vartext.PaddingLeft = leftPadding;
                panel.Append(vartext);
                if (topPadding < 100)
                {
                    topPadding += 20;
                } else
                {
                    topPadding = 0;
                    leftPadding += 150;
                }
            }
        }
    }
}