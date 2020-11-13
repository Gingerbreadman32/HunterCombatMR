using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public HunterCombatPlayer Player
        {
            get
            {
                return _currentPlayer;
            }
            set
            {
                _currentPlayer = value;
            }
        }

        private HunterCombatPlayer _currentPlayer;

        private UIAutoScaleTextTextPanel<string> _modeswitch;

        private UIPanel _bufferpanel;

        private UIPanel _layerpanel;

        private UIPanel _testlistpanel;
        private UIList _testlist;

        private UIPanel _animationtoolpanel;

        private UIElement _framegroup;
        private UIAutoScaleTextTextPanel<string> _reverseframe;
        private UIText _framenum;
        private UIAutoScaleTextTextPanel<string> _advanceframe;

        private UIElement _animationgroup;
        private UIAutoScaleTextTextPanel<string> _playpause;
        private UIAutoScaleTextTextPanel<string> _stop;
        private UIAutoScaleTextTextPanel<string> _looptype;

        private UIElement _timinggroup;
        private UIText _frametotal;
        private UIText _currentframetime;
        private UIAutoScaleTextTextPanel<string> _addtimebutton;
        private UIAutoScaleTextTextPanel<string> _subtimebutton;
        private UIAutoScaleTextTextPanel<string> _defaulttimebutton;

        private UIAutoScaleTextTextPanel<string> _savebutton;
        private UIAutoScaleTextTextPanel<string> _loadbutton;

        private UIAutoScaleTextTextPanel<string> _onionskinbutton;

        private int saveTimer = 0;
        private int loadTimer = 0;
        private const int SAVETIMERMAX = 120;

        private TextBox _animationname;

        public override void OnInitialize()
        {
            // Mode Switch
            _modeswitch = new UIAutoScaleTextTextPanel<string>(HunterCombatMR.Instance.EditorInstance.CurrentEditMode.GetDescription())
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
            _modeswitch.OnClick += ModeSwitch;

            Append(_modeswitch);

            _bufferpanel = new UIPanel();
            _bufferpanel.Width.Set(0f, 0.18f);
            _bufferpanel.Height.Set(0f, 0.1f);
            _bufferpanel.BackgroundColor = Microsoft.Xna.Framework.Color.Red;
            _bufferpanel.BackgroundColor.A = 50;
            _bufferpanel.HAlign = 0.65f;
            _bufferpanel.VAlign = 0.05f;
            _bufferpanel.OverflowHidden = true;
            Append(_bufferpanel);

            _layerpanel = new UIPanel();
            _layerpanel.Width.Set(0, 0.2f);
            _layerpanel.Height.Set(0, 0.15f);
            _layerpanel.BackgroundColor = Microsoft.Xna.Framework.Color.Blue;
            _layerpanel.BackgroundColor.A = 50;
            _layerpanel.HAlign = 0f;
            _layerpanel.VAlign = 0.5f;
            _layerpanel.OverflowHidden = true;
            Append(_layerpanel);

            var underLayerPanel = new StyleDimension(_layerpanel.GetDimensions().Y + _layerpanel.GetDimensions().Height, 0);

            var testlistpanelleft = new StyleDimension(_layerpanel.Width.Pixels, 0);

            _testlistpanel = new UIPanel();
            _testlistpanel.Width.Set(0, 0.15f);
            _testlistpanel.Height.Set(0, 0.15f);
            _testlistpanel.BackgroundColor = Microsoft.Xna.Framework.Color.YellowGreen;
            _testlistpanel.BackgroundColor.A = 50;
            _testlistpanel.Top = new StyleDimension(_layerpanel.GetDimensions().Y, 0f);
            _testlistpanel.Left = new StyleDimension(_layerpanel.GetDimensions().X + _layerpanel.GetDimensions().Width, 0);
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

            _animationtoolpanel = new UIPanel()
            {
                Width = new StyleDimension(0, 0.35f),
                Height = new StyleDimension(50f, 0f),
                Top = underLayerPanel
            };
            _animationtoolpanel.BackgroundColor.A = 50;

            float panelPercent = 0f;

            _framegroup = new UIElement();
            _framegroup.Width.Set(0, 0.1f);
            _framegroup.Height.Set(0, 1f);

            panelPercent += _framegroup.Width.Percent;

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

            _framegroup.Append(_reverseframe);
            _framegroup.Append(_framenum);
            _framegroup.Append(_advanceframe);

            _animationtoolpanel.Append(_framegroup);

            _animationgroup = new UIElement();
            _animationgroup.Width.Set(0, 0.35f);
            _animationgroup.Height.Set(50f, 0f);
            _animationgroup.Left = new StyleDimension(0, panelPercent);

            panelPercent += _animationgroup.Width.Percent;

            _playpause = new UIAutoScaleTextTextPanel<string>("Play")
            {
                TextColor = Color.Green,
                Width = new StyleDimension(0, 0.25f),
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
                Width = new StyleDimension(0f, 0.25f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(0f, 0.25f)
            }.WithFadedMouseOver();
            _stop.OnClick += StopAnimation;

            _looptype = new UIAutoScaleTextTextPanel<string>("Loop")
            {
                TextColor = Color.White,
                Width = new StyleDimension(0f, 0.50f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(0f, 0.5f)
            }.WithFadedMouseOver();
            _looptype.OnClick += LoopTypeChange;

            _animationgroup.Append(_playpause);
            _animationgroup.Append(_stop);
            _animationgroup.Append(_looptype);

            _animationtoolpanel.Append(_animationgroup);

            _timinggroup = new UIElement();
            _timinggroup.Width.Set(0, 0.35f);
            _timinggroup.Height.Set(50f, 0);
            _timinggroup.Left = new StyleDimension(0, panelPercent);

            panelPercent += _timinggroup.Width.Percent;

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

            _timinggroup.Append(_subtimebutton);
            _timinggroup.Append(_currentframetime);
            _timinggroup.Append(_addtimebutton);
            _timinggroup.Append(_defaulttimebutton);
            _timinggroup.Append(_frametotal);

            _animationtoolpanel.Append(_timinggroup);

            // Save and Load Buttons
            _savebutton = new UIAutoScaleTextTextPanel<string>("Save")
            {
                TextColor = Color.White,
                Width = new StyleDimension(0, 0.1f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(0, panelPercent)
            }.WithFadedMouseOver();
            _savebutton.OnClick += SaveAnimation;
            _animationtoolpanel.Append(_savebutton);

            panelPercent += _savebutton.Width.Percent;

            _loadbutton = new UIAutoScaleTextTextPanel<string>("Load")
            {
                TextColor = Color.White,
                Width = new StyleDimension(0f, 0.1f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(0, panelPercent)
            }.WithFadedMouseOver();
            _loadbutton.OnClick += LoadAnimation;
            _animationtoolpanel.Append(_loadbutton);

            panelPercent += _loadbutton.Width.Percent;

            Append(_animationtoolpanel);

            // Misc. Tool Panel

            UIPanel othertoolpanel = new UIPanel()
            {
                Width = new StyleDimension(0, 0.35f),
                Height = new StyleDimension(50f, 0f),
                Top = new StyleDimension(underLayerPanel.Pixels + _animationtoolpanel.GetDimensions().Height, 0)
            };
            othertoolpanel.BackgroundColor.A = 50;

            float panelPercent2 = 0f;

            UIElement playergroup = new UIElement()
            {
                Width = new StyleDimension(0, 0.25f),
                Height = new StyleDimension(0, 1f),
                Left = new StyleDimension(0, panelPercent2),
                OverflowHidden = false
            };
            panelPercent2 += playergroup.Width.Percent;

            var flipbutton = new UIAutoScaleTextTextPanel<string>("\u27F7")
            {
                Height = new StyleDimension(0, 1f),
                Width = new StyleDimension(35f, 0)
            }.WithFadedMouseOver();
            flipbutton.OnClick += (evt, list) =>
            {
                if (_currentPlayer != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None)) { _currentPlayer.player.ChangeDir(-_currentPlayer.player.direction); }
            };

            playergroup.Append(flipbutton);
            othertoolpanel.Append(playergroup);

            UIElement drawgroup = new UIElement()
            {
                Width = new StyleDimension(0, 0.25f),
                Height = new StyleDimension(0, 1f),
                Left = new StyleDimension(0, panelPercent2),
                OverflowHidden = false
            };
            panelPercent2 += drawgroup.Width.Percent;

            _onionskinbutton = new UIAutoScaleTextTextPanel<string>("\u259A")
            {
                Height = new StyleDimension(0, 1f),
                Width = new StyleDimension(35f, 0)
            }.WithFadedMouseOver();
            _onionskinbutton.OnClick += (evt, list) =>
            {
                if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None)) { HunterCombatMR.Instance.EditorInstance.ToggleOnionSkin(); }
            };

            drawgroup.Append(_onionskinbutton);
            othertoolpanel.Append(drawgroup);

            Append(othertoolpanel);

            // Renameable Animation Name Text Box
            _animationname = new TextBox("Animation Name", 64, true, true);
            _animationname.Left.Set(0f, 0f);
            _animationname.Top.Set(-_layerpanel.GetDimensions().Height, 0.5f);
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
            if (_currentPlayer != null)
            {
                /*
                // Top Line
                DrawLine(spriteBatch, _currentPlayer.player.position, 0, _currentPlayer.player.width);

                // Bottom Line
                DrawLine(spriteBatch, _currentPlayer.player.BottomLeft, 0, _currentPlayer.player.width);

                // Left Line
                DrawLine(spriteBatch, _currentPlayer.player.position, MathHelper.PiOver2, _currentPlayer.player.height);

                // Right Line
                DrawLine(spriteBatch, _currentPlayer.player.TopRight, MathHelper.PiOver2, _currentPlayer.player.height);

                // Vertical Midline
                DrawLine(spriteBatch, _currentPlayer.player.Top, MathHelper.PiOver2, _currentPlayer.player.height);
                */

                if (HunterCombatMR.Instance.EditorInstance.OnionSkin)
                    _onionskinbutton.BorderColor = Color.White;
                else
                    _onionskinbutton.BorderColor = Color.Black;
            }
            base.Draw(spriteBatch);
        }

        private void DrawLine(SpriteBatch spriteBatch,
            Vector2 position,
            float direction,
            float length)
        {
            Texture2D SimpleTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            SimpleTexture.SetData(new[] { Color.White });

            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, 2f);
            spriteBatch.Draw(SimpleTexture, position - Main.screenPosition, null, Color.Red, direction, origin, scale, SpriteEffects.None, 0);
        }

        private void SaveAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized && saveTimer == 0)
            {
                FileSaveStatus saveStatus;
                string animName = _currentPlayer.CurrentAnimation.Name;
                string oldName;

                if (_animationname.Interacting)
                    return;
                else if (!string.IsNullOrWhiteSpace(_animationname.Text) && _animationname.Text != animName)
                {
                    oldName = animName;
                    animName = _animationname.Text;
                    saveStatus = HunterCombatMR.Instance.FileManager.SaveAnimationNewName(_currentPlayer.CurrentAnimation, animName, true);

                    if (HunterCombatMR.Instance.LoadedAnimations.Any(x => x.Name.Equals(oldName)))
                    {
                        HunterCombatMR.Instance.LoadedAnimations.Remove(HunterCombatMR.Instance.LoadedAnimations.First(x => x.Name.Equals(oldName)));
                        _testlist.Clear();
                    }
                }
                else
                {
                    saveStatus = HunterCombatMR.Instance.FileManager.SaveAnimation(_currentPlayer.CurrentAnimation, true);
                }

                if (saveStatus == FileSaveStatus.Saved)
                {
                    saveTimer++;
                    HunterCombatMR.Instance.LoadAnimation(_currentPlayer.CurrentAnimation.LayerData.ParentType, animName);
                    _currentPlayer.SetCurrentAnimation(HunterCombatMR.Instance.LoadedAnimations.First(x => x.Name.Equals(animName)));
                }
                else if (saveStatus == FileSaveStatus.Error)
                {
                    throw new System.Exception($"Animation could not save!");
                }
                else
                {
                    Main.NewText(saveStatus.ToString());
                }

                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void LoadAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized && loadTimer == 0)
            {
                var loaded = HunterCombatMR.Instance.LoadAnimation(_currentPlayer.CurrentAnimation.LayerData.ParentType, _currentPlayer.CurrentAnimation.Name);
                if (loaded)
                {
                    _currentPlayer.SetCurrentAnimation(HunterCombatMR.Instance.LoadedAnimations.First(x => x.Name.Equals(_currentPlayer.CurrentAnimation.Name)));
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

            if (_currentPlayer.CurrentAnimation != null && HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
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

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
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

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
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

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
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

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                _currentPlayer.CurrentAnimation.Animation.AdvanceToNextKeyFrame();
                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void ReverseFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (_currentPlayer.CurrentAnimation != null && !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && _currentPlayer.CurrentAnimation.Animation.IsInitialized)
            {
                _currentPlayer.CurrentAnimation?.Animation.ReverseToPreviousKeyFrame();
                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void ModeSwitch(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode))
            {
                HunterCombatMR.Instance.EditorInstance.CurrentEditMode = EditorMode.None;
            }
            else
            {
                HunterCombatMR.Instance.EditorInstance.CurrentEditMode++;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _modeswitch.SetText(HunterCombatMR.Instance.EditorInstance.CurrentEditMode.GetDescription());

            if (Main.gameMenu || _currentPlayer == null)
            {
                return;
            }

            // Editor Check
            var inEditor = !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None);

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
            if (Main.mouseLeft && !_animationname.IsMouseHovering)
                _animationname.StopInteracting();

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
                    activelayers.Add($"{layer.Name} - X: {layer.Frames[currentFrame].Position.X}" +
                        $" Y: {layer.Frames[currentFrame].Position.Y}");
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
                foreach (var animation in HunterCombatMR.Instance.LoadedAnimations)
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

                if (_testlist._items.Any())
                    _testlist._items.ForEach(x => (x as UIAutoScaleTextTextPanel<string>).TextColor = Color.White);

                if (_currentPlayer?.CurrentAnimation != null)
                {
                    UIAutoScaleTextTextPanel<string> listSelected = (UIAutoScaleTextTextPanel<string>)_testlist._items.FirstOrDefault(x => _currentPlayer.CurrentAnimation.Name.Equals((x as UIAutoScaleTextTextPanel<string>).Text));
                    listSelected.TextColor = Color.Aqua;
                    if (!_currentPlayer.CurrentAnimation.Equals(HunterCombatMR.Instance.LoadedAnimations.FirstOrDefault(x => x.Name.Equals(_animationname.Text))))
                    {
                        listSelected.TextColor = Color.OrangeRed;
                    }
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

            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                _currentPlayer.SetCurrentAnimation(HunterCombatMR.Instance.LoadedAnimations.First(x => x.Name.Equals(button.Text)));
                Main.PlaySound(SoundID.MenuTick);
            }
        }

        private void HighlightFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode))
            {
                UIText text = (UIText)listeningElement;
                HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Clear();
                HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Add(text.Text.Split('-')[0].Trim());
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
                if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Any(x => x.Equals(text.Split('-')[0].Trim())))
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