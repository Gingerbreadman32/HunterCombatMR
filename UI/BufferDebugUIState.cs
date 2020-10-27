using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
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

        private UIPanel _framepanel;
        private UIAutoScaleTextTextPanel<string> _reverseframe;
        private UIText _framenum;
        private UIAutoScaleTextTextPanel<string> _advanceframe;

        private UIPanel _animationpanel;
        private UIAutoScaleTextTextPanel<string> _playpause;
        private UIAutoScaleTextTextPanel<string> _stop;
        private UIAutoScaleTextTextPanel<string> _looptype;

        private UIPanel _timingpanel;
        private UIText _frametotal;
        private UIText _currentframetime;
        private UIAutoScaleTextTextPanel<string> _addtimebutton;
        private UIAutoScaleTextTextPanel<string> _subtimebutton;
        private UIAutoScaleTextTextPanel<string> _defaulttimebutton;

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
            _layerpanel.HAlign = 0f;
            _layerpanel.VAlign = 0.5f;
            Append(_layerpanel);

            _framepanel = new UIPanel();
            _framepanel.Width.Set(100, 0);
            _framepanel.Height.Set(50, 0);
            _framepanel.BackgroundColor = Color.Transparent;
            _framepanel.BorderColor = Color.Transparent;
            _framepanel.Left = new StyleDimension(0, 0);
            _framepanel.Top = new StyleDimension(_layerpanel.Height.Pixels + 500f, 0);

            _reverseframe = new UIAutoScaleTextTextPanel<string>("<")
            {
                TextColor = Color.White,
                Width = new StyleDimension(25f, 0),
                Height =
                {
                    Pixels = 40f
                },
                HAlign = 0f
            }.WithFadedMouseOver();
            _reverseframe.OnClick += ReverseFrame;

            _framenum = new UIText("0")
            {
                HAlign = 0.5f
            };

            _advanceframe = new UIAutoScaleTextTextPanel<string>(">")
            {
                TextColor = Color.White,
                Width = new StyleDimension(25f, 0),
                Height =
                {
                    Pixels = 40f
                },
                HAlign = 1f
            }.WithFadedMouseOver();
            _advanceframe.OnClick += AdvanceFrame;

            _framepanel.Append(_reverseframe);
            _framepanel.Append(_framenum);
            _framepanel.Append(_advanceframe);

            Append(_framepanel);

            _animationpanel = new UIPanel();
            _animationpanel.Width.Set(200, 0);
            _animationpanel.Height.Set(50, 0);
            _animationpanel.BackgroundColor = Color.Transparent;
            _animationpanel.BorderColor = Color.Transparent;
            _animationpanel.Left = new StyleDimension(_framepanel.Width.Pixels, 0);
            _animationpanel.Top = new StyleDimension(_layerpanel.Height.Pixels + 500f, 0);

            _playpause = new UIAutoScaleTextTextPanel<string>("Play")
            {
                TextColor = Color.Green,
                Width = new StyleDimension(50f, 0),
                Height =
                {
                    Pixels = 40f
                },
                HAlign = 0f
            }.WithFadedMouseOver();
            _playpause.OnClick += PlayPauseAnimation;

            _stop = new UIAutoScaleTextTextPanel<string>("Stop")
            {
                TextColor = Color.Red,
                Width = new StyleDimension(40f, 0),
                Height =
                {
                    Pixels = 40f
                },
                HAlign = 0.4f
            }.WithFadedMouseOver();
            _stop.OnClick += StopAnimation;

            _looptype = new UIAutoScaleTextTextPanel<string>("Loop")
            {
                TextColor = Color.White,
                Width = new StyleDimension(75f, 0),
                Height =
                {
                    Pixels = 40f
                },
                HAlign = 1f
            }.WithFadedMouseOver();
            _looptype.OnClick += LoopTypeChange;

            _animationpanel.Append(_playpause);
            _animationpanel.Append(_stop);
            _animationpanel.Append(_looptype);

            Append(_animationpanel);

            _timingpanel = new UIPanel();
            _timingpanel.Width.Set(250, 0);
            _timingpanel.Height.Set(50, 0);
            _timingpanel.BackgroundColor = Color.Transparent;
            _timingpanel.BorderColor = Color.Transparent;
            _timingpanel.Left = new StyleDimension(_animationpanel.Width.Pixels + _framepanel.Width.Pixels, 0);
            _timingpanel.Top = new StyleDimension(_layerpanel.Height.Pixels + 500f, 0);

            _subtimebutton = new UIAutoScaleTextTextPanel<string>("-")
            {
                TextColor = Color.White,
                Width = new StyleDimension(25f, 0),
                Height =
                {
                    Pixels = 25f
                },
                HAlign = 0f
            }.WithFadedMouseOver();
            _subtimebutton.OnClick += SubtractFrameTime;

            _currentframetime = new UIText("0")
            {
                HAlign = 0.10f
            };

            _addtimebutton = new UIAutoScaleTextTextPanel<string>("+")
            {
                TextColor = Color.White,
                Width = new StyleDimension(25f, 0),
                Height =
                {
                    Pixels = 25f
                },
                HAlign = 0.20f
            }.WithFadedMouseOver();
            _addtimebutton.OnClick += AddFrameTime;

            _defaulttimebutton = new UIAutoScaleTextTextPanel<string>("Default")
            {
                TextColor = Color.White,
                Width = new StyleDimension(75f, 0),
                Height =
                {
                    Pixels = 25f
                },
                HAlign = 0.5f
            }.WithFadedMouseOver();
            _defaulttimebutton.OnClick += DefaultFrameTime;

            _frametotal = new UIText("0")
            {
                HAlign = 0.75f
            };

            _timingpanel.Append(_subtimebutton);
            _timingpanel.Append(_currentframetime);
            _timingpanel.Append(_addtimebutton);
            _timingpanel.Append(_defaulttimebutton);
            _timingpanel.Append(_frametotal);

            Append(_timingpanel);

            var buttonEA = new UIAutoScaleTextTextPanel<string>(HunterCombatMR.EditorInstance.CurrentEditMode.GetDescription())
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
            buttonEA.OnClick += ModeSwitch;

            Append(buttonEA);
        }

        private void AddFrameTime(UIMouseEvent evt, UIElement listeningElement)
        {
            FrameTimeLogic(1);
        }

        private void SubtractFrameTime(UIMouseEvent evt, UIElement listeningElement)
        {
            FrameTimeLogic(-1);
        }

        private void DefaultFrameTime(UIMouseEvent evt, UIElement listeningElement)
        {
            FrameTimeLogic(0, true);
        }

        private void FrameTimeLogic(int amount,
            bool setFrame = false)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                if (!_currentPlayer.CurrentAnimation.Animation.IsPlaying)
                {
                    var currentKeyframe = _currentPlayer.CurrentAnimation.Animation.GetCurrentKeyFrameIndex();

                    if (amount == 0 && setFrame)
                        amount = _currentPlayer.CurrentAnimation.LayerData.KeyFrameProfile.DefaultKeyFrameSpeed;
                    else if (amount + _currentPlayer.CurrentAnimation.Animation.GetCurrentKeyFrame().FrameLength <= 0)
                        return;

                    HunterCombatMR.AnimationKeyFrameManager.AdjustKeyFrameLength(_currentPlayer.CurrentAnimation.Animation, 
                        _currentPlayer.CurrentAnimation.Animation.GetCurrentKeyFrameIndex(), 
                        amount,
                        !setFrame);

                    _currentPlayer.CurrentAnimation.Animation.SetCurrentFrame(_currentPlayer.CurrentAnimation.Animation.KeyFrames[currentKeyframe].StartingFrameIndex);
                }

                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void PlayPauseAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                if (!_currentPlayer.CurrentAnimation.Animation.IsPlaying)
                    _currentPlayer.CurrentAnimation.Play();
                else
                    _currentPlayer.CurrentAnimation.Pause();

                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void StopAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                if (_currentPlayer.CurrentAnimation.Animation.InProgress)
                    _currentPlayer.CurrentAnimation.Stop();

                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void LoopTypeChange(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                if (!_currentPlayer.CurrentAnimation.Animation.LoopMode.Equals(LoopStyle.PlayPause))
                    _currentPlayer.CurrentAnimation.Animation.SetLoopMode(_currentPlayer.CurrentAnimation.Animation.LoopMode + 1);
                else
                    _currentPlayer.CurrentAnimation.Animation.SetLoopMode(0);

                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void AdvanceFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                _currentPlayer.CurrentAnimation.Animation.AdvanceToNextKeyFrame();
                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void ReverseFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                _currentPlayer.CurrentAnimation?.Animation.ReverseToPreviousKeyFrame();
                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void ModeSwitch(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode))
            {
                HunterCombatMR.EditorInstance.CurrentEditMode = EditorMode.None;
                (listeningElement as UIAutoScaleTextTextPanel<string>).TextColor = Color.White;
            }
            else if (HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.ViewMode))
            {
                HunterCombatMR.EditorInstance.CurrentEditMode = EditorMode.EditMode;
                (listeningElement as UIAutoScaleTextTextPanel<string>).TextColor = Color.Green;
            }
            else
            {
                HunterCombatMR.EditorInstance.CurrentEditMode = EditorMode.ViewMode;
                (listeningElement as UIAutoScaleTextTextPanel<string>).TextColor = Color.Orange;
            }

        (listeningElement as UIAutoScaleTextTextPanel<string>).SetText(HunterCombatMR.EditorInstance.CurrentEditMode.GetDescription());
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

            if (_currentPlayer?.CurrentAnimation != null && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                var currentFrame = _currentPlayer.CurrentAnimation.Animation.GetCurrentKeyFrameIndex();
                foreach (var layer in _currentPlayer.CurrentAnimation.LayerData.Layers)
                {
                    var framelayerindex = $"{layer.Name}-{currentFrame}";
                    var nudgePos = (_currentPlayer.LayerPositions.ContainsKey(framelayerindex)) ? _currentPlayer.LayerPositions[framelayerindex] : new Vector2();
                    activelayers.Add($"{layer.Name} - X: {layer.Frames[currentFrame].Position.X + nudgePos.X}" +
                        $" Y: {layer.Frames[currentFrame].Position.Y + nudgePos.Y}");
                }
                ListVariables(_layerpanel, activelayers);

                _playpause.SetText((_currentPlayer.CurrentAnimation.Animation.IsPlaying) ? "Pause" : "Play");
                _looptype.SetText(_currentPlayer.CurrentAnimation.Animation.LoopMode.ToString());
            } else
            {
                _layerpanel.RemoveAllChildren();
            }

            ListVariables(_bufferpanel, buffers);

            if (!HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                var frametext = _currentPlayer.CurrentAnimation?.Animation.GetCurrentKeyFrameIndex().ToString() ?? "0";
                _framenum.SetText(frametext);

                var framenumtext = _currentPlayer.CurrentAnimation?.Animation.GetCurrentKeyFrame().FrameLength.ToString() ?? "0";
                _currentframetime.SetText(framenumtext);

                var totalframenumtext = _currentPlayer.CurrentAnimation?.Animation.TotalFrames.ToString() ?? "0";
                _frametotal.SetText(totalframenumtext);
            }
        }

        private void HighlightFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode))
            {
                UIText text = (UIText)listeningElement;
                HunterCombatMR.EditorInstance.HighlightedLayers.Clear();
                HunterCombatMR.EditorInstance.HighlightedLayers.Add(text.Text.Split('-')[0].Trim());
            }
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
                if (HunterCombatMR.EditorInstance.HighlightedLayers.Any(x => x.Equals(text.Split('-')[0].Trim())))
                    vartext.TextColor = Color.Red;

                vartext.OnClick += HighlightFrame;
                panel.Append(vartext);
                if (topPadding < 100)
                {
                    topPadding += 20;
                }
                else
                {
                    topPadding = 0;
                    leftPadding += 150;
                }
            }
        }
    }
}