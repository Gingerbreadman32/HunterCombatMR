using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Globalization;
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

        private UIPanel _testlistpanel;
        private UIList _testlist;

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

        private UIAutoScaleTextTextPanel<string> _savebutton;
        private UIAutoScaleTextTextPanel<string> _loadbutton;

        private int saveTimer = 0;
        private int loadTimer = 0;
        private const int SAVETIMERMAX = 120;

        private TextBox _animationname;

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
            _defaulttimebutton.OnClick += ((evt, listen) => FrameTimeLogic(0, true));

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

            var testlistpanelleft = new StyleDimension(_layerpanel.Width.Pixels, 0);

            _testlistpanel = new UIPanel();
            _testlistpanel.Width.Set(125, 0);
            _testlistpanel.Height.Set(150, 0);
            _testlistpanel.BackgroundColor = Microsoft.Xna.Framework.Color.YellowGreen;
            _testlistpanel.BackgroundColor.A = 50;
            _testlistpanel.VAlign = 0.5f;
            _testlistpanel.Left = testlistpanelleft;
            Append(_testlistpanel);

            int top = 20;

            _testlist = new UIList();
            _testlist.Top.Pixels = top;
            _testlist.Width.Set(-25f, 1f);
            _testlist.Height.Set(-top, 1f);
            _testlist.ListPadding = 6f;
            _testlist.OverflowHidden = true;
            _testlistpanel.Append(_testlist);

            var interfaceLayerListScrollbar = new UIScrollbar();
            interfaceLayerListScrollbar.SetView(100f, 1000f);
            interfaceLayerListScrollbar.Top.Pixels = top;
            interfaceLayerListScrollbar.Height.Set(-20f, 1f);
            interfaceLayerListScrollbar.HAlign = 1f;
            _testlistpanel.Append(interfaceLayerListScrollbar);
            _testlist.SetScrollbar(interfaceLayerListScrollbar);

            // Save and Load Buttons
            _savebutton = new UIAutoScaleTextTextPanel<string>("Save")
            {
                TextColor = Color.White,
                Width = new StyleDimension(70f, 0f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(_animationpanel.Width.Pixels + _framepanel.Width.Pixels + _timingpanel.Width.Pixels, 0),
                Top = new StyleDimension(_layerpanel.Height.Pixels + 500f, 0)
            }.WithFadedMouseOver();
            _savebutton.OnClick += SaveAnimation;
            Append(_savebutton);

            _loadbutton = new UIAutoScaleTextTextPanel<string>("Load")
            {
                TextColor = Color.White,
                Width = new StyleDimension(70f, 0f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(_animationpanel.Width.Pixels + _framepanel.Width.Pixels + _timingpanel.Width.Pixels + _savebutton.Width.Pixels, 0),
                Top = new StyleDimension(_layerpanel.Height.Pixels + 500f, 0)
            }.WithFadedMouseOver();
            _loadbutton.OnClick += LoadAnimation;
            Append(_loadbutton);

            // Renameable Animation Name Text Box
            _animationname = new TextBox("Animation Name", 64, true, true);
            _animationname.Left.Set(0f, 0f);
            _animationname.Top.Set(-_layerpanel.Height.Pixels, 0.5f);
            _animationname.TextColor = Color.White;
            _animationname.OnClick += Interact;
            _animationname.ForbiddenCharacters = new char[] 
                { (char)32, '>', '<', ':', '"', '/', '\u005C', '|', '?', '*', '.'  };
            Append(_animationname);
        }

        private void Interact(UIMouseEvent evt, UIElement listeningElement)
        {
            TextBox element = (TextBox)listeningElement;
            element.StartInteracting();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        private void SaveAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized && saveTimer == 0)
            {
                var saveStatus = HunterCombatMR.FileManager.SaveAnimation(_currentPlayer.CurrentAnimation, true);

                if (saveStatus == FileSaveStatus.Saved)
                {
                    saveTimer++;
                    HunterCombatMR.LoadAnimation(_currentPlayer.CurrentAnimation.LayerData.ParentType, _currentPlayer.CurrentAnimation.Name);
                    _currentPlayer.SetCurrentAnimation(HunterCombatMR.LoadedAnimations.First(x => x.Name.Equals(_currentPlayer.CurrentAnimation.Name)));
                }
                else
                {
                    throw new System.Exception($"Animation could not save! Save Status: {saveStatus}");
                }

                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void LoadAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized && loadTimer == 0)
            {
                var loaded = HunterCombatMR.LoadAnimation(_currentPlayer.CurrentAnimation.LayerData.ParentType, _currentPlayer.CurrentAnimation.Name);
                if (loaded)
                {
                    _currentPlayer.SetCurrentAnimation(HunterCombatMR.LoadedAnimations.First(x => x.Name.Equals(_currentPlayer.CurrentAnimation.Name)));
                    _currentPlayer.CurrentAnimation.Animation.SetCurrentFrame(0);
                    loadTimer++;
                    Main.PlaySound(SoundID.MenuTick);
                }
            }
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
                    var defaultSpeed = false;

                    if (amount == 0 && setFrame)
                        defaultSpeed = true;
                    else if (amount + _currentPlayer.CurrentAnimation.Animation.GetCurrentKeyFrame().FrameLength <= 0)
                        return;

                    _currentPlayer.CurrentAnimation.UpdateKeyFrameLength(currentKeyframe, amount, setFrame, defaultSpeed);

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
                    _currentPlayer.CurrentAnimation.UpdateLoopType(_currentPlayer.CurrentAnimation.Animation.LoopMode + 1);
                else
                    _currentPlayer.CurrentAnimation.UpdateLoopType(0);

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
            // Editor Check
            var inEditor = !HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None);

            // Save Button
            if (saveTimer > 0 && saveTimer < SAVETIMERMAX)
            {
                saveTimer++;
                _savebutton.SetText("Saved!");
                _savebutton.BackgroundColor = Color.DarkBlue;
            }
            else if (saveTimer >= SAVETIMERMAX)
            {
                saveTimer = 0;
                _savebutton.SetText("Save");
                _savebutton.BackgroundColor = UICommon.DefaultUIBlue;
            }

            // Load Button
            if (loadTimer > 0 && loadTimer < SAVETIMERMAX)
            {
                loadTimer++;
                _loadbutton.SetText("Loaded!");
                _loadbutton.BackgroundColor = Color.DarkBlue;
            }
            else if (loadTimer >= SAVETIMERMAX)
            {
                loadTimer = 0;
                _loadbutton.SetText("Load");
                _loadbutton.BackgroundColor = UICommon.DefaultUIBlue;
            }

            // Get Player
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            // Animation Name Text Box
            _animationname.Hidden = (!inEditor || _currentPlayer == null || _currentPlayer.CurrentAnimation == null);
            if (_currentPlayer?.CurrentAnimation != null)
            {
                _animationname.DefaultText = _currentPlayer.CurrentAnimation.Name;
            }

            // Buffer Window
            var buffers = new List<string>();
            foreach (var buffer in _currentPlayer.InputBufferInfo.BufferedComboInputs)
            {
                buffers.Add($"{buffer.Input.ToString()} - {buffer.FramesSinceBuffered}");
            }

            ListVariables(_bufferpanel, buffers);

            // Layer Window
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
                foreach (var layer in _currentPlayer.CurrentAnimation.LayerData.Layers.OrderBy(x => x.Frames[currentFrame].LayerDepth))
                {
                    var framelayerindex = $"{layer.Name}-{currentFrame}";
                    var nudgePos = (_currentPlayer.LayerPositions.ContainsKey(framelayerindex)) ? _currentPlayer.LayerPositions[framelayerindex] : new Vector2();
                    activelayers.Add($"{layer.Name} - X: {layer.Frames[currentFrame].Position.X + nudgePos.X}" +
                        $" Y: {layer.Frames[currentFrame].Position.Y + nudgePos.Y}");
                }
                ListVariables(_layerpanel, activelayers);

                _playpause.SetText((_currentPlayer.CurrentAnimation.Animation.IsPlaying) ? "Pause" : "Play");
                _looptype.SetText(_currentPlayer.CurrentAnimation.Animation.LoopMode.ToString());
            }
            else
            {
                _layerpanel.RemoveAllChildren();
            }

            if (inEditor)
            {
                // Current Keyframe Text
                var frametext = (_currentPlayer.CurrentAnimation?.Animation.GetCurrentKeyFrameIndex() + 1).ToString() ?? "0";
                _framenum.SetText(frametext);

                // Current Keyframe Timing Text
                var framenumtext = _currentPlayer.CurrentAnimation?.Animation.GetCurrentKeyFrame().FrameLength.ToString() ?? "0";
                _currentframetime.SetText(framenumtext);

                // Total Amount of Animation Frames
                var totalframenumtext = _currentPlayer.CurrentAnimation?.Animation.TotalFrames.ToString() ?? "0";
                _frametotal.SetText(totalframenumtext);

                // Animation List Window
                foreach (var animation in HunterCombatMR.LoadedAnimations)
                {
                    UIAutoScaleTextTextPanel<string> animationButton = new UIAutoScaleTextTextPanel<string>(animation.Name)
                    {
                        TextColor = Color.White,
                        Width = new StyleDimension(-10f, 1f),
                        Height =
                        {
                            Pixels = 40f
                        },
                        TextScale = 1f
                    }.WithFadedMouseOver();
                    animationButton.OnClick += SelectAnimation;
                    if (!_testlist._items.Any(x => (x as UIAutoScaleTextTextPanel<string>).Text.Equals(animationButton.Text)))
                        _testlist.Add(animationButton);
                }

                if (_currentPlayer?.CurrentAnimation != null)
                {
                    UIAutoScaleTextTextPanel<string> listSelected = (UIAutoScaleTextTextPanel<string>)_testlist._items.FirstOrDefault(x => _currentPlayer.CurrentAnimation.Name.Equals((x as UIAutoScaleTextTextPanel<string>).Text));
                    listSelected.TextColor = Color.Aqua;
                }
            }
            else
            {
                _testlist.Clear();
            }
        }

        private void SelectAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            UIAutoScaleTextTextPanel<string> button = (UIAutoScaleTextTextPanel<string>)listeningElement;
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (!HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                _currentPlayer.SetCurrentAnimation(HunterCombatMR.LoadedAnimations.First(x => x.Name.Equals(button.Text)));
                Main.PlaySound(SoundID.MenuTick);
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