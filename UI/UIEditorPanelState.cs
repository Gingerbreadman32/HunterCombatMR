using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.Enumerations;
using HunterCombatMR.UI.AnimationTimeline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
    public class UIEditorPanelState
        : UIState
    {
        #region Public Properties

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

        #endregion Public Properties

        #region Private Fields

        private const int SAVETIMERMAX = 120;
        private UIAutoScaleTextTextPanel<string> _addtimebutton;
        private UIElement _animationgroup;
        private TextBoxBase _animationname;
        private Timeline _animationTimeline;
        private UIPanel _animationtoolpanel;
        private UIPanel _bufferpanel;
        private UIText _currentframetime;
        private UIText _currentlayertextureframe;
        private HunterCombatPlayer _currentPlayer;
        private IList<Texture2D> _textures;

        private UIAutoScaleTextTextPanel<string> _defaulttimebutton;
        private UIElement _framegroup;
        private UIText _frametotal;
        private UIList _layerlist;
        private UIPanel _layerpanel;
        private UIAutoScaleTextTextPanel<string> _loadbutton;
        private UIAutoScaleTextTextPanel<string> _onionskinbutton;
        private UIAutoScaleTextTextPanel<string> _savebutton;
        private UIAutoScaleTextTextPanel<string> _subtimebutton;
        private UIAutoScaleTextTextPanel<string> _layertexturebutton;
        private UIList _testlist;
        private UIPanel _testlistpanel;
        private UIElement _timinggroup;
        private int loadTimer = 0;
        private int saveTimer = 0;
        private LayerInformationPanel _layerInfoPanel;

        #endregion Private Fields

        #region Internal Properties

        internal IEnumerable<LayerText> CurrentAnimationLayers
        {
            get
            {
                return _layerlist?._items.Select(x => (x as LayerText)) ?? new List<LayerText>();
            }
        }

        #endregion Internal Properties

        #region Public Methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_currentPlayer != null)
            {
                if (HunterCombatMR.Instance.EditorInstance.OnionSkin)
                    _onionskinbutton.BorderColor = Color.White;
                else
                    _onionskinbutton.BorderColor = Color.Black;
            }
            base.Draw(spriteBatch);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

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
            _layerpanel.Width.Set(0, 0.15f);
            _layerpanel.Height.Set(0, 0.15f);
            _layerpanel.BackgroundColor = Microsoft.Xna.Framework.Color.Blue;
            _layerpanel.BackgroundColor.A = 50;
            _layerpanel.HAlign = 0f;
            _layerpanel.VAlign = 0.5f;
            _layerpanel.OverflowHidden = true;
            Append(_layerpanel);

            _layerlist = new UIList();
            _layerlist.Width.Set(0, 1f);
            _layerlist.Height.Set(0, 1f);
            _layerlist.ListPadding = 6f;
            _layerlist.OverflowHidden = true;
            _layerpanel.Append(_layerlist);

            var layerListScrollbar = new UIScrollbar();
            layerListScrollbar.SetView(100f, 1000f);
            layerListScrollbar.Top.Pixels = 10f;
            layerListScrollbar.Height.Set(-20f, 1f);
            layerListScrollbar.HAlign = 1f;
            _layerpanel.Append(layerListScrollbar);
            _layerlist.SetScrollbar(layerListScrollbar);

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

            var lasttexframe = new UIAutoScaleTextTextPanel<string>("<")
            {
                TextColor = Color.White,
                Width = new StyleDimension(25f, 0),
                Height =
                {
                    Pixels = 40f
                },
                HAlign = 0f
            }.WithFadedMouseOver();
            lasttexframe.OnClick += (evt, list) => ButtonAction(PreviousTextureFrame, evt, list, EditorModePreset.InEditor, true);

            _currentlayertextureframe = new UIText("0")
            {
                HAlign = 0.5f
            };

            var nexttexframe = new UIAutoScaleTextTextPanel<string>(">")
            {
                TextColor = Color.White,
                Width = new StyleDimension(25f, 0),
                Height =
                {
                    Pixels = 40f
                },
                HAlign = 1f
            }.WithFadedMouseOver();
            nexttexframe.OnClick += (evt, list) => ButtonAction(NextTextureFrame, evt, list, EditorModePreset.InEditor, true);

            _framegroup.Append(nexttexframe);
            _framegroup.Append(_currentlayertextureframe);
            _framegroup.Append(lasttexframe);

            _animationtoolpanel.Append(_framegroup);

            _animationgroup = new UIElement();
            _animationgroup.Width.Set(0, 0.35f);
            _animationgroup.Height.Set(50f, 0f);
            _animationgroup.Left = new StyleDimension(0, panelPercent);

            panelPercent += _animationgroup.Width.Percent;

            _layertexturebutton = new UIAutoScaleTextTextPanel<string>("")
            {
                TextColor = Color.White,
                Width = new StyleDimension(0, 1f),
                Height = new StyleDimension(0, 1f)
            }.WithFadedMouseOver();
            _layertexturebutton.OnClick += (evt, list) => ButtonAction(CycleTexture, evt, list, EditorModePreset.EditOnly, true);

            _animationgroup.Append(_layertexturebutton);
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
            _subtimebutton.OnClick += ((evt, listen) => ButtonAction((x, y) => FrameTimeLogic(-1), evt, listen, EditorModePreset.EditOnly, true));

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
            _addtimebutton.OnClick += ((evt, listen) => ButtonAction((x, y) => FrameTimeLogic(1), evt, listen, EditorModePreset.EditOnly, true));

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
            _defaulttimebutton.OnClick += ((evt, listen) => ButtonAction((x, y) => FrameTimeLogic(0, true), evt, listen, EditorModePreset.EditOnly, true));

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
            _savebutton.OnClick += (evt, list) => ButtonAction(SaveAnimation, evt, list, EditorModePreset.InEditor, true);
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
            _loadbutton.OnClick += (evt, list) => ButtonAction(ReloadAnimation, evt, list, EditorModePreset.InEditor, true);
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
            flipbutton.OnClick += (evt, list) => ButtonAction((e, l) =>
            {
                _currentPlayer.player.ChangeDir(-_currentPlayer.player.direction);
            }, evt, list, EditorModePreset.InEditor, false);

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

            UIAutoScaleTextTextPanel<string> duplicatebutton = new UIAutoScaleTextTextPanel<string>("New")
            {
                TextColor = Color.White,
                Width = new StyleDimension(0, 0.1f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(0, panelPercent2)
            }.WithFadedMouseOver();
            duplicatebutton.OnClick += (evt, list) => ButtonAction((x, y) =>
            {
                var newAnim = HunterCombatMR.Instance.DuplicateAnimation(_currentPlayer?.CurrentAnimation);
                _currentPlayer.SetCurrentAnimation(HunterCombatMR.Instance.GetLoadedAnimation(newAnim), true);
                _animationname.Text = newAnim;
            }, evt, list, EditorModePreset.EditOnly, true);
            panelPercent2 += duplicatebutton.Width.Percent;
            othertoolpanel.Append(duplicatebutton);

            UIAutoScaleTextTextPanel<string> deletebutton = new UIAutoScaleTextTextPanel<string>("Delete")
            {
                TextColor = Color.White,
                Width = new StyleDimension(0, 0.1f),
                Height =
                {
                    Pixels = 40f
                },
                Left = new StyleDimension(0, panelPercent2)
            }.WithFadedMouseOver();
            deletebutton.OnClick += (evt, list) => ButtonAction((x, y) =>
            {
                AnimationEngine.Models.Animation animToDelete = _currentPlayer?.CurrentAnimation;
                _currentPlayer.SetCurrentAnimation(null);
                HunterCombatMR.Instance.DeleteAnimation(animToDelete);
                _animationname.Text = string.Empty;
                _testlist.Clear();
            }, evt, list, EditorModePreset.EditOnly, true);
            panelPercent2 += deletebutton.Width.Percent;
            othertoolpanel.Append(deletebutton);

            Append(othertoolpanel);

            _animationTimeline = new Timeline()
            {
                Top = new StyleDimension(underLayerPanel.Pixels + _animationtoolpanel.GetDimensions().Height + othertoolpanel.GetDimensions().Height + 2f, 0),
                Left = new StyleDimension(8f, 0f)
            };

            Append(_animationTimeline);

            // Renameable Animation Name Text Box
            _animationname = new PlainTextBox("Animation Name", HunterCombatMR.AnimationNameMax, true, true, characterPerms: InputPermissions.FileSafe)
            {
                Top = new StyleDimension(-_layerpanel.GetDimensions().Height, 0.5f),
                TextColor = Color.White
            };
            Append(_animationname);

            _layerInfoPanel = LayerInformationPanel.Default(new StyleDimension(-300f, 1f), new StyleDimension(0f, 0.5f));
            Append(_layerInfoPanel);
        }

        public void PostSetupContent()
        {
            _textures = ModContentLoadingService.GetTexturesFromPath("Textures/SnS/Limbs/").Select(x => x.Value).ToList();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

            // Layer Window
            var layers = PlayerHooks.GetDrawLayers(_currentPlayer.player);

            if (HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing == null
                || !HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing.IsAnimationInitialized())
            {
                _layerlist.Clear();
            }

            if (inEditor)
            {
                var currentKeyFrame = _currentPlayer.CurrentAnimation?.AnimationData.GetCurrentKeyFrameIndex() ?? 0;
                // Current Keyframe Timing Text
                var framenumtext = _currentPlayer.CurrentAnimation?.AnimationData.GetCurrentKeyFrame().FrameLength.ToString() ?? "0";
                _currentframetime.SetText(framenumtext);

                // Total Amount of Animation KeyFrames
                var totalframenumtext = _currentPlayer.CurrentAnimation?.AnimationData.TotalFrames.ToString() ?? "0";
                _frametotal.SetText(totalframenumtext);

                if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Count() == 1)
                {
                    // Current Layer's Texture Frame
                    int keyFrame = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing
                        ?.AnimationData.GetCurrentKeyFrameIndex()
                        ?? 0;
                    string layertexframe = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing
                        ?.LayerData.GetTextureFrameAtKeyFrameForLayer(keyFrame, HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Single()).ToString()
                        ?? "";
                    _currentlayertextureframe.SetText(layertexframe);

                    // Current Layer's Texture
                    var layer = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing
                        ?.GetLayer(HunterCombatMR.Instance.EditorInstance.HighlightedLayers.SingleOrDefault());
                    string layertexture = layer
                        ?.Texture.ToString().Split('/')[4]
                        ?? "";
                    _layertexturebutton.SetText(layertexture);

                    if (_layerInfoPanel.Layer != layer || _layerInfoPanel.KeyFrame != keyFrame)
                    {
                        _layerInfoPanel.SetLayerAndKeyFrame(layer, keyFrame);
                    }

                    if (_layerInfoPanel.IsCollapsed && _layerInfoPanel.Layer != null)
                        _layerInfoPanel.Reveal();
                }
                else
                {
                    _currentlayertextureframe.SetText("");
                    _layertexturebutton.SetText("");
                    if (!_layerInfoPanel.IsCollapsed)
                        _layerInfoPanel.Collapse();
                }

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
                    animationButton.OnClick += (evt, list) => ButtonAction(SelectAnimation, evt, list, EditorModePreset.InEditor);
                    if (!_testlist._items.Any(x => (x as UIAutoScaleTextTextPanel<string>).Text.Equals(animationButton.Text)))
                        _testlist.Add(animationButton);
                }

                if (_testlist._items.Any())
                    _testlist._items.ForEach(x => (x as UIAutoScaleTextTextPanel<string>).TextColor = Color.White);

                if (_currentPlayer?.CurrentAnimation != null)
                {
                    UIAutoScaleTextTextPanel<string> listSelected = (UIAutoScaleTextTextPanel<string>)_testlist._items.FirstOrDefault(x => _currentPlayer.CurrentAnimation.Name.Equals((x as UIAutoScaleTextTextPanel<string>).Text));
                    if (listSelected != null)
                    {
                        if (_currentPlayer.CurrentAnimation.IsModified)
                        {
                            listSelected.TextColor = Color.OrangeRed;
                        }
                        else
                        {
                            listSelected.TextColor = Color.Aqua;
                        }
                    }
                }

                if (HunterCombatMR.Instance.EditorInstance.AnimationEdited)
                    DisplayLayers(_currentPlayer?.CurrentAnimation);
            }
            else
            {
                _testlist.Clear();
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal void ButtonAction(Action<UIMouseEvent, UIElement> action,
            UIMouseEvent evt,
            UIElement listen,
            EditorMode[] modeRestriction = null,
            bool animationNeeded = false)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (modeRestriction == null || modeRestriction.Contains(HunterCombatMR.Instance.EditorInstance.CurrentEditMode) &&
                    (!animationNeeded || (animationNeeded && _currentPlayer.CurrentAnimation != null)))
            {
                action(evt, listen);
                Main.PlaySound(SoundID.MenuTick);
                DisplayLayers(_currentPlayer.CurrentAnimation);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void DisplayLayers(AnimationEngine.Models.Animation animation)
        {
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = false;
            _layerlist.Clear();

            if (animation == null)
                return;

            var currentKeyFrame = animation.AnimationData.GetCurrentKeyFrameIndex();
            foreach (var layer in animation.LayerData.Layers.OrderBy(x => x.KeyFrames[currentKeyFrame].LayerDepth))
            {
                if (!_layerlist._items.Any(x => (x as LayerText).Layer.Equals(layer)))
                {
                    _layerlist.Add(new LayerText(animation, layer.Name, currentKeyFrame, LayerTextInfo.None));
                }
            }
        }

        private void FrameTimeLogic(int amount,
                    bool setFrame = false)
        {
            if (_currentPlayer.CurrentAnimation.IsAnimationInitialized() && !_currentPlayer.CurrentAnimation.AnimationData.IsPlaying)
            {
                var currentKeyframe = _currentPlayer.CurrentAnimation.AnimationData.GetCurrentKeyFrameIndex();
                var defaultSpeed = false;

                if (amount == 0 && setFrame)
                    defaultSpeed = true;
                else if (amount + _currentPlayer.CurrentAnimation.AnimationData.GetCurrentKeyFrame().FrameLength <= 0)
                    return;

                _currentPlayer.CurrentAnimation.UpdateKeyFrameLength(currentKeyframe, amount, setFrame, defaultSpeed);

                _currentPlayer.CurrentAnimation.AnimationData.SetCurrentFrame(_currentPlayer.CurrentAnimation.AnimationData.KeyFrames[currentKeyframe].StartingFrameIndex);
            }
        }

        private void NextTextureFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing?.IsAnimationInitialized() ?? false)
            {
                var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
                if (layers.Count() == 1)
                {
                    AnimationEngine.Models.Animation anim = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing;
                    var layer = anim.LayerData.Layers.Single(x => x.Name.Equals(layers.Single()));
                    var key = anim.AnimationData.GetCurrentKeyFrameIndex();
                    if (layer.GetSpriteTextureFrameTotal() - 1 > layer.KeyFrames[key].SpriteFrame)
                    {
                        anim.UpdateLayerTextureFrame(layer, layer.KeyFrames[key].SpriteFrame + 1);
                    }
                }
            }
        }

        private void PreviousTextureFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing?.IsAnimationInitialized() ?? false)
            {
                var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
                if (layers.Count() == 1)
                {
                    AnimationEngine.Models.Animation anim = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing;
                    var layer = anim.LayerData.Layers.Single(x => x.Name.Equals(layers.Single()));
                    var key = anim.AnimationData.GetCurrentKeyFrameIndex();
                    if (layer.KeyFrames[key].SpriteFrame > 0)
                    {
                        anim.UpdateLayerTextureFrame(layer, layer.KeyFrames[key].SpriteFrame - 1);
                    }
                }
            }
        }

        private void ReloadAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer.CurrentAnimation.IsAnimationInitialized() && loadTimer == 0)
            {
                var currentFrame = _currentPlayer.CurrentAnimation.AnimationData.CurrentFrame;
                var loaded = HunterCombatMR.Instance.LoadAnimationFile(_currentPlayer.CurrentAnimation.AnimationType, _currentPlayer.CurrentAnimation.Name, true);
                if (loaded)
                {
                    _currentPlayer.SetCurrentAnimation(HunterCombatMR.Instance.LoadedAnimations.First(x => x.Name.Equals(_currentPlayer.CurrentAnimation.Name)));

                    if (currentFrame > _currentPlayer.CurrentAnimation.AnimationData.GetFinalFrame() && currentFrame <= 0)
                        currentFrame = 0;

                    _currentPlayer.CurrentAnimation.AnimationData.SetCurrentFrame(currentFrame);
                    HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing = _currentPlayer.CurrentAnimation;
                    loadTimer++;
                }
            }
        }

        private void SaveAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_currentPlayer.CurrentAnimation.IsAnimationInitialized() && saveTimer == 0)
            {
                FileSaveStatus saveStatus;
                int currentFrame = _currentPlayer.CurrentAnimation.AnimationData.CurrentFrame;
                string animName = _currentPlayer.CurrentAnimation.Name;
                string oldName;

                if (_animationname.Interacting)
                {
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(_animationname.Text) && _animationname.Text != animName)
                {
                    oldName = animName;
                    animName = _animationname.Text;
                    _currentPlayer.CurrentAnimation.Uninitialize();
                    saveStatus = HunterCombatMR.Instance.FileManager.SaveCustomAnimation(_currentPlayer.CurrentAnimation, animName, true);

                    if (_currentPlayer.CurrentAnimation.IsInternal)
                        HunterCombatMR.Instance.FileManager.SaveInternalAnimation(_currentPlayer.CurrentAnimation, animName, true);

                    if (HunterCombatMR.Instance.LoadedAnimations.Any(x => x.Name.Equals(oldName)))
                    {
                        HunterCombatMR.Instance.LoadedAnimations.Remove(HunterCombatMR.Instance.LoadedAnimations.First(x => x.Name.Equals(oldName)));
                        _testlist.Clear();
                    }
                }
                else
                {
                    _currentPlayer.CurrentAnimation.Uninitialize();
                    saveStatus = HunterCombatMR.Instance.FileManager.SaveCustomAnimation(_currentPlayer.CurrentAnimation, overwrite: true);

                    if (_currentPlayer.CurrentAnimation.IsInternal)
                        HunterCombatMR.Instance.FileManager.SaveInternalAnimation(_currentPlayer.CurrentAnimation, overwrite: true);
                }

                if (saveStatus == FileSaveStatus.Saved)
                {
                    saveTimer++;
                    HunterCombatMR.Instance.LoadAnimationFile(_currentPlayer.CurrentAnimation.AnimationType, animName, true);
                    _currentPlayer.SetCurrentAnimation(HunterCombatMR.Instance.LoadedAnimations.First(x => x.Name.Equals(animName)));
                    HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing = _currentPlayer.CurrentAnimation;

                    if (currentFrame > _currentPlayer.CurrentAnimation.AnimationData.GetFinalFrame() && currentFrame <= 0)
                        currentFrame = 0;

                    _currentPlayer.CurrentAnimation.AnimationData.SetCurrentFrame(currentFrame);
                }
                else if (saveStatus == FileSaveStatus.Error)
                {
                    _currentPlayer.CurrentAnimation.Initialize();
                    throw new System.Exception($"Animation could not save!");
                }
                else
                {
                    Main.NewText(saveStatus.ToString());
                    _currentPlayer.CurrentAnimation.Initialize();
                }
            }
        }

        private void SelectAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            var newAnim = HunterCombatMR.Instance.LoadedAnimations.FirstOrDefault(x => x.Name.Equals((listeningElement as UIAutoScaleTextTextPanel<string>).Text));
            _currentPlayer?.SetCurrentAnimation(newAnim, newAnim.IsModified);
            HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing = _currentPlayer.CurrentAnimation;
            _animationname.Text = _currentPlayer?.CurrentAnimation?.Name;
            _animationTimeline.SetAnimation(HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing);
        }

        private void CycleTexture(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing?.IsAnimationInitialized() ?? false)
            {
                var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
                if (layers.Count() == 1)
                {
                    AnimationEngine.Models.Animation anim = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing;
                    var layer = anim.GetLayer(layers.Single());
                    int textureIndex = _textures.IndexOf(layer.Texture);

                    if (_textures.Count() - 1 > textureIndex)
                        anim.UpdateLayerTexture(layer, _textures[textureIndex + 1]);
                    else
                        anim.UpdateLayerTexture(layer, _textures[0]);
                }
            }
        }

        #endregion Private Methods
    }
}