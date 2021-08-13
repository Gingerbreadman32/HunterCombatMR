using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
using HunterCombatMR.UI.Elements;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    internal class DebugUI
        : UIState
    {
        private UIAutoScaleTextTextPanel<string> _modeSwitch;
        private PlayerInformationPanel _playerInfoPanel;
        private List<PopUpButton> _popUps;
        private UIPanel _bufferpanel;
        private UIList _bufferList;

        public HunterCombatPlayer Player { get; set; }

        internal DebugUI()
        {
            _popUps = new List<PopUpButton>();
        }

        internal IEnumerable<LayerText> Layers { get; private set; }

        public override void OnInitialize()
        {
            base.OnInitialize();

            // Mode Switch
            _modeSwitch = new UIAutoScaleTextTextPanel<string>(HunterCombatMR.Instance.EditorInstance.CurrentEditMode.GetDescription())
            {
                TextColor = Color.White,
                Width = new StyleDimension(200f, 0),
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
            _modeSwitch.OnClick += (evt, list) => ModeSwitch(evt, list);

            _playerInfoPanel = new PlayerInformationPanel();
            _playerInfoPanel.Initialize();

            Append(_modeSwitch);
            Append(_playerInfoPanel);

            _bufferpanel = new UIPanel();
            _bufferpanel.Width.Set(0f, 0.18f);
            _bufferpanel.Height.Set(0f, 0.1f);
            _bufferpanel.BackgroundColor = Microsoft.Xna.Framework.Color.Red;
            _bufferpanel.BackgroundColor.A = 50;
            _bufferpanel.HAlign = 0.65f;
            _bufferpanel.VAlign = 0.05f;
            _bufferpanel.OverflowHidden = true;
            _bufferList = new UIList() { Width = StyleDimension.Fill, Height = StyleDimension.Fill };
            _bufferpanel.Append(_bufferList);
            Append(_bufferpanel);
        }

        public void RemoveAllChildrenOfType<T>() where T : UIElement
        {
            Elements.RemoveAll(x => x.GetType().IsInstanceOfType(typeof(T)));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Player == null || Player != Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>())
            {
                Player = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();
                _playerInfoPanel.SetPlayer(Player);
            }

            _modeSwitch.SetText(HunterCombatMR.Instance.EditorInstance.CurrentEditMode.GetDescription());


            // Buffer Window
            _bufferList.Clear();
            if (!ComponentManager.HasComponent<InputComponent>(Player.EntityReference))
                return;

            var inputs = ComponentManager.GetEntityComponent<InputComponent>(Player.EntityReference).BufferedInputs;
            
            foreach (var buffer in inputs.ToArray())
            {
                _bufferList.Add(new UIText($"{buffer.Input.ToString()} - {buffer.FramesSinceBuffered} - {buffer.FramesHeld}", 0.5f));
            }
        }

        internal void UpdateActiveLayers(IEnumerable<LayerText> layers)
        {
            Layers = layers;
            var highlighted = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;

            if (highlighted.Count() == 1 && layers.Any(x => x.LayerRef.Layer.Name.Equals(highlighted.FirstOrDefault())))
            {
                var selected = layers.FirstOrDefault(y => y.LayerRef.Layer.Name.Equals(highlighted.FirstOrDefault()));
                if (!_popUps.Any(x => x.AttachedElement == selected))
                {
                    ClearPopUps();

                    var upButton = new PopUpButton('\u25B2', 40f, 40f, selected, new Vector2(120f, -50f)).WithFadedMouseOver();
                    upButton.OnClick += (evt, list) =>
                        {
                            //selected.Layer.UpdateLayerDepth(-1, currentKeyFrame, layers.Select(x => x.Layer));
                            HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
                        };
                    _popUps.Add(upButton);

                    var enableButton = new PopUpButton((selected.LayerRef.Layer.Name != null) ? '\u2713' : '\u2715', 40f, 40f, selected, new Vector2(120f, 0f)).WithFadedMouseOver();
                    enableButton.OnClick += (evt, list) =>
                    {
                        //selected.Layer.ToggleVisibility(currentKeyFrame);
                        HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
                    };
                    _popUps.Add(enableButton);

                    var downButton = new PopUpButton('\u25BC', 40f, 40f, selected, new Vector2(120f, 50f)).WithFadedMouseOver();
                    downButton.OnClick += (evt, list) =>
                    {
                        //selected.Layer.UpdateLayerDepth(1, currentKeyFrame, layers.Select(x => x.Layer));
                        HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
                    };
                    _popUps.Add(downButton);

                    _popUps.ForEach(x => Append(x));
                }
            }
            else
            {
                ClearPopUps();
            }
        }

        private void ClearPopUps()
        {
            RemoveAllChildrenOfType<PopUpButton>();
            _popUps.Clear();
        }

        private void ModeSwitch(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.AnimationEdit))
            {
                HunterCombatMR.Instance.EditorInstance.CurrentEditMode = EditorMode.None;
            }
            else
            {
                HunterCombatMR.Instance.EditorInstance.CurrentEditMode = EditorMode.AnimationEdit;
            }

            Main.PlaySound(SoundID.MenuTick);
        }
    }
}