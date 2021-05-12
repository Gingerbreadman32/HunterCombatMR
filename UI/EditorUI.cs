using HunterCombatMR.AnimationEngine.Enumerations;
using HunterCombatMR.AnimationEngine.Extensions;
using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.Enumerations;
using HunterCombatMR.UI.AnimationTimeline;
using HunterCombatMR.UI.Elements;
using HunterCombatMR.Utilities;
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
    public class EditorUI
        : UIState
    {
        private const int SAVETIMERMAX = 120;

        private UIAutoScaleTextTextPanel<string> _addtimebutton;

        private UIElement _animationgroup;

        private TextBoxBase _animationname;

        private Timeline _animationTimeline;

        private UIPanel _animationtoolpanel;

        private UIText _currentframetime;

        private UIText _currentlayertextureframe;

        private HunterCombatPlayer _currentPlayer;

        private UIAutoScaleTextTextPanel<string> _defaulttimebutton;

        private UIElement _framegroup;

        private UIText _frametotal;

        private LayerInformationPanel _layerInfoPanel;

        private UIList _layerlist;

        private UIPanel _layerpanel;

        private UIAutoScaleTextTextPanel<string> _layertexturebutton;

        private UIAutoScaleTextTextPanel<string> _loadbutton;

        private UIAutoScaleTextTextPanel<string> _onionskinbutton;

        private UIAutoScaleTextTextPanel<string> _savebutton;

        private UIAutoScaleTextTextPanel<string> _subtimebutton;

        private UIList _testlist;

        private UIPanel _testlistpanel;

        private IList<Texture2D> _textures;

        private UIElement _timinggroup;

        private int loadTimer = 0;

        private int saveTimer = 0;

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

        internal IEnumerable<LayerText> CurrentAnimationLayers
        {
            get
            {
                return _layerlist?._items.Select(x => (x as LayerText)) ?? new List<LayerText>();
            }
        }

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
            _layertexturebutton.OnClick += (evt, list) => ButtonAction(CycleTexture, evt, list, EditorModePreset.InEditor, true);

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
            _subtimebutton.OnClick += ((evt, listen) => ButtonAction((x, y) => FrameTimeLogic(-1), evt, listen, EditorModePreset.InEditor, true));

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
            _addtimebutton.OnClick += ((evt, listen) => ButtonAction((x, y) => FrameTimeLogic(1), evt, listen, EditorModePreset.InEditor, true));

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
            _defaulttimebutton.OnClick += ((evt, listen) => ButtonAction((x, y) => FrameTimeLogic(0, true), evt, listen, EditorModePreset.InEditor, true));

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
                var newAnim = HunterCombatMR.Instance.Content.DuplicateContentInstance(_currentPlayer?.CurrentAnimation);
                EditorInstanceUtils.EditingAnimation = ContentUtils.GetInstance<PlayerAnimation>(newAnim);
                _animationname.Text = newAnim;
            }, evt, list, EditorModePreset.InEditor, true);
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
                IAnimation animToDelete = _currentPlayer?.CurrentAnimation;
                _currentPlayer.SetCurrentAnimation(null);
                HunterCombatMR.Instance.DeleteAnimation(animToDelete);
                _animationname.Text = string.Empty;
                _testlist.Clear();
            }, evt, list, EditorModePreset.InEditor, true);
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
            _animationname = new PlainTextBox("Animation Name", 64, true, true, characterPerms: InputPermissions.FileSafe)
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
            _animationname.Hidden = (!inEditor || _currentPlayer == null || EditorInstanceUtils.EditingAnimation == null);
            if (_currentPlayer?.CurrentAnimation != null)
            {
                _animationname.DefaultText = EditorInstanceUtils.EditingAnimation.Name;
            }
            if (Main.mouseLeft && !_animationname.IsMouseHovering)
                _animationname.StopInteracting();

            // Layer Window
            var layers = PlayerHooks.GetDrawLayers(_currentPlayer.player);

            if (EditorInstanceUtils.EditingAnimation == null
                || !EditorInstanceUtils.EditingAnimation.IsInitialized)
            {
                _layerlist.Clear();
            }

            if (inEditor)
            {
                var currentKeyFrame = EditorInstanceUtils.EditingAnimation?.AnimationData.CurrentKeyFrameIndex ?? 0;
                // Current Keyframe Timing Text
                var framenumtext = EditorInstanceUtils.EditingAnimation?.AnimationData.CurrentKeyFrame.FrameLength.ToString() ?? "0";
                _currentframetime.SetText(framenumtext);

                // Total Amount of Animation KeyFrames
                var totalframenumtext = EditorInstanceUtils.EditingAnimation?.AnimationData.TotalFrames.ToString() ?? "0";
                _frametotal.SetText(totalframenumtext);

                if (HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Count() == 1)
                {
                    // Current Layer's Texture Frame
                    int keyFrame = EditorInstanceUtils.EditingAnimation
                        ?.AnimationData.CurrentKeyFrameIndex
                        ?? 0;
                    string layertexframe = EditorInstanceUtils.EditingAnimation
                        ?.LayerData.GetTextureFrameAtKeyFrameForLayer(keyFrame, HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Single()).ToString()
                        ?? "";
                    _currentlayertextureframe.SetText(layertexframe);

                    // Current Layer's Texture
                    var layer = EditorInstanceUtils.EditingAnimation
                        ?.LayerData.GetLayer(HunterCombatMR.Instance.EditorInstance.HighlightedLayers.SingleOrDefault());
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
                foreach (var animation in HunterCombatMR.Instance.Content.GetContentList<PlayerAnimation>())
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
                    UIAutoScaleTextTextPanel<string> listSelected = (UIAutoScaleTextTextPanel<string>)_testlist._items.FirstOrDefault(x => EditorInstanceUtils.EditingAnimation.Name.Equals((x as UIAutoScaleTextTextPanel<string>).Text));
                    if (listSelected != null)
                    {
                        if (EditorInstanceUtils.EditingAnimation.IsModified)
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

        internal void ButtonAction(Action<UIMouseEvent, UIElement> action,
            UIMouseEvent evt,
            UIElement listen,
            EditorMode[] modeRestriction = null,
            bool animationNeeded = false)
        {
            if (_currentPlayer == null)
                _currentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

            if (modeRestriction == null || modeRestriction.Contains(HunterCombatMR.Instance.EditorInstance.CurrentEditMode) &&
                    (!animationNeeded || (animationNeeded && EditorInstanceUtils.EditingAnimation != null)))
            {
                action(evt, listen);
                Main.PlaySound(SoundID.MenuTick);
                DisplayLayers(EditorInstanceUtils.EditingAnimation);
            }
        }

        private void CycleTexture(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EditorInstanceUtils.EditingAnimation?.IsInitialized ?? false)
            {
                var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
                if (layers.Count() == 1)
                {
                    IAnimation anim = EditorInstanceUtils.EditingAnimation;
                    var layer = anim.LayerData.GetLayer(layers.Single());
                    int textureIndex = _textures.IndexOf(layer.Texture);

                    if (_textures.Count() - 1 > textureIndex)
                        layer.SetTexture(_textures[textureIndex + 1]);
                    else
                        layer.SetTexture(_textures[0]);
                }
            }
        }

        private void DisplayLayers(IAnimation animation)
        {
            HunterCombatMR.Instance.EditorInstance.AnimationEdited = false;
            _layerlist.Clear();

            if (animation == null)
                return;

            var currentKeyFrame = animation.AnimationData.CurrentKeyFrameIndex;
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
            if (EditorInstanceUtils.EditingAnimation.IsInitialized
                && EditorInstanceUtils.EditingAnimation.AnimationData.Flags.HasFlag(AnimatorFlags.Locked))
            {
                FrameIndex currentKeyframeIndex = EditorInstanceUtils.EditingAnimation.AnimationData.CurrentKeyFrameIndex;
                KeyFrame keyFrame = EditorInstanceUtils.EditingAnimation.AnimationData.KeyFrames[currentKeyframeIndex];

                if (amount == 0 && setFrame)
                    amount = EditorInstanceUtils.EditingAnimation.LayerData.KeyFrameProfile.DefaultKeyFrameLength;
                else if (amount + EditorInstanceUtils.EditingAnimation.AnimationData.CurrentKeyFrame.FrameLength <= 0)
                    return;

                FrameLength newLength = (setFrame) ? amount : (int)(amount + keyFrame.FrameLength);

                EditorInstanceUtils.EditingAnimation.UpdateKeyFrameLength(currentKeyframeIndex, newLength);
                EditorInstanceUtils.EditingAnimation.AnimationData.CurrentFrame = keyFrame.StartingFrameIndex;
            }
        }

        private void NextTextureFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EditorInstanceUtils.EditingAnimation?.IsInitialized ?? false)
            {
                var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
                if (layers.Count() == 1)
                {
                    IAnimation anim = EditorInstanceUtils.EditingAnimation;
                    var layer = anim.LayerData.Layers.Single(x => x.Name.Equals(layers.Single()));
                    var key = anim.AnimationData.CurrentKeyFrameIndex;
                    if (layer.GetTotalTextureFrames() - 1 > layer.KeyFrames[key].SpriteFrame)
                    {
                        layer.SetTextureFrame(key, layer.KeyFrames[key].SpriteFrame + 1);
                    }
                }
            }
        }

        private void PreviousTextureFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EditorInstanceUtils.EditingAnimation?.IsInitialized ?? false)
            {
                var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
                if (layers.Count() == 1)
                {
                    IAnimation anim = EditorInstanceUtils.EditingAnimation;
                    var layer = anim.LayerData.Layers.Single(x => x.Name.Equals(layers.Single()));
                    var key = anim.AnimationData.CurrentKeyFrameIndex;
                    if (layer.KeyFrames[key].SpriteFrame > 0)
                    {
                        layer.SetTextureFrame(key, layer.KeyFrames[key].SpriteFrame - 1);
                    }
                }
            }
        }

        private void ReloadAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EditorInstanceUtils.EditingAnimation.IsInitialized && loadTimer == 0)
            {
                var currentFrame = EditorInstanceUtils.EditingAnimation.AnimationData.CurrentFrame;
                var loaded = HunterCombatMR.Instance.Content.LoadAnimationFile(EditorInstanceUtils.EditingAnimation.AnimationType, EditorInstanceUtils.EditingAnimation.Name, true);
                if (loaded)
                {
                    EditorInstanceUtils.EditingAnimation =
                        HunterCombatMR.Instance.Content.GetContentInstance<PlayerAnimation>(EditorInstanceUtils.EditingAnimation.Name);

                    if (currentFrame > EditorInstanceUtils.EditingAnimation.AnimationData.FinalFrame && currentFrame <= 0)
                        currentFrame = FrameIndex.Zero;

                    EditorInstanceUtils.EditingAnimation.AnimationData.CurrentFrame = currentFrame;
                    _animationTimeline.SetAnimation(EditorInstanceUtils.EditingAnimation);
                    loadTimer++;
                }
            }
        }

        private void SaveAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EditorInstanceUtils.EditingAnimation.IsInitialized && saveTimer == 0)
            {
                FileSaveStatus saveStatus;
                FrameIndex currentFrame = EditorInstanceUtils.EditingAnimation.AnimationData.CurrentFrame;
                string animName = EditorInstanceUtils.EditingAnimation.Name;
                string oldName;

                if (_animationname.Interacting)
                {
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(_animationname.Text) && _animationname.Text != animName)
                {
                    oldName = animName;
                    animName = _animationname.Text;
                    EditorInstanceUtils.EditingAnimation.Uninitialize();
                    saveStatus = HunterCombatMR.Instance.FileManager.SaveCustomAnimation(EditorInstanceUtils.EditingAnimation as PlayerAnimation, animName, true);

                    if (EditorInstanceUtils.EditingAnimation.IsStoredInternally)
                        HunterCombatMR.Instance.FileManager.SaveInternalAnimation(EditorInstanceUtils.EditingAnimation as PlayerAnimation, animName, true);

                    if (HunterCombatMR.Instance.Content.CheckContentInstanceByName<PlayerAnimation>(oldName))
                    {
                        HunterCombatMR.Instance.Content.DeleteContentInstance<PlayerAnimation>(oldName);
                        _testlist.Clear();
                    }
                }
                else
                {
                    EditorInstanceUtils.EditingAnimation.Uninitialize();
                    saveStatus = HunterCombatMR.Instance.FileManager.SaveCustomAnimation(EditorInstanceUtils.EditingAnimation as PlayerAnimation, overwrite: true);

                    if (EditorInstanceUtils.EditingAnimation.IsStoredInternally)
                        HunterCombatMR.Instance.FileManager.SaveInternalAnimation(EditorInstanceUtils.EditingAnimation as PlayerAnimation, overwrite: true);
                }

                if (saveStatus == FileSaveStatus.Saved)
                {
                    saveTimer++;
                    HunterCombatMR.Instance.Content.LoadAnimationFile(EditorInstanceUtils.EditingAnimation.AnimationType, animName, true);
                    EditorInstanceUtils.EditingAnimation =
                        HunterCombatMR.Instance.Content.GetContentInstance<PlayerAnimation>(animName);

                    if (currentFrame > EditorInstanceUtils.EditingAnimation.AnimationData.FinalFrame && currentFrame <= 0)
                        currentFrame = FrameIndex.Zero;

                    EditorInstanceUtils.EditingAnimation.AnimationData.CurrentFrame = currentFrame;
                    _animationTimeline.SetAnimation(EditorInstanceUtils.EditingAnimation);
                }
                else if (saveStatus == FileSaveStatus.Error)
                {
                    EditorInstanceUtils.EditingAnimation.Initialize();
                    throw new System.Exception($"Animation could not save!");
                }
                else
                {
                    Main.NewText(saveStatus.ToString());
                    EditorInstanceUtils.EditingAnimation.Initialize();
                }
            }
        }

        private void SelectAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            var newAnim = HunterCombatMR.Instance.Content.GetContentInstance<PlayerAnimation>((listeningElement as UIAutoScaleTextTextPanel<string>).Text);
            EditorInstanceUtils.EditingAnimation = newAnim;
            _animationname.Text = EditorInstanceUtils.EditingAnimation?.Name;
            _animationTimeline.SetAnimation(EditorInstanceUtils.EditingAnimation);
        }
    }
}