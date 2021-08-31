using HunterCombatMR.Builders.Animation;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Models.UI.AnimationTimeline;
using HunterCombatMR.Models.UI.Elements;
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

namespace HunterCombatMR.Models.UI
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
        private HunterCombatPlayer CurrentPlayer;
        private int loadTimer = 0;

        private int saveTimer = 0;

        internal IEnumerable<LayerText> CurrentAnimationLayers
        {
            get
            {
                return _layerlist?._items.Select(x => x as LayerText) ?? new List<LayerText>();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentPlayer != null)
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
            _layerpanel.BackgroundColor = Color.Blue;
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
            _testlistpanel.BackgroundColor = Color.YellowGreen;
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
            _layertexturebutton.OnClick += (evt, list) => ButtonAction(CycleTexture, evt, list, new EditorMode[] { }, true); // Disabling

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
            _subtimebutton.OnClick += (evt, listen) => ButtonAction((x, y) => FrameTimeLogic(-1), evt, listen, EditorModePreset.InEditor, true);

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
            _addtimebutton.OnClick += (evt, listen) => ButtonAction((x, y) => FrameTimeLogic(1), evt, listen, EditorModePreset.InEditor, true);

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
            _defaulttimebutton.OnClick += (evt, listen) => ButtonAction((x, y) => FrameTimeLogic(0), evt, listen, new EditorMode[] { }, true); // Gonna just remove this

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
                CurrentPlayer.player.ChangeDir(-CurrentPlayer.player.direction);
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
                AnimationBuilder animToDelete = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing;
                CurrentPlayer.EntityReference.RemoveComponent<AnimationComponent>();
                //HunterCombatMR.Instance.Content.DeleteContentInstance(animToDelete);
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
            _textures = ContentUtils.GetTexturesFromPath("Textures/SnS/Limbs/").Select(x => x.Value).ToList();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.gameMenu || Main.LocalPlayer == null)
            {
                return;
            }

            // Editor Check
            var inEditor = !HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None);

            if (!inEditor)
            {
                _testlist.Clear();
                return;
            }

            if (CurrentPlayer == null || !CurrentPlayer.IsMainPlayer)
                CurrentPlayer = Main.LocalPlayer.GetModPlayer<HunterCombatPlayer>();

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

            // Animation Name Text Box
            _animationname.Hidden = !inEditor || EditorUtils.EditingAnimation == null;
            _animationname.DefaultText = EditorUtils.EditingAnimation?.Name;

            if (Main.mouseLeft && !_animationname.IsMouseHovering)
                _animationname.StopInteracting();

            // Layer Window
            var layers = PlayerHooks.GetDrawLayers(CurrentPlayer.player);

            if (EditorUtils.EditingAnimation == null)
            {
                _layerlist.Clear();
            }

            if (!CurrentPlayer.EntityReference.TryGetComponent(out EntityStateComponent component))
                return;

            // Animation List Window
            foreach (var animation in component.AnimationSet)
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

            if (!CurrentPlayer.EntityReference.HasComponent<AnimationComponent>() || EditorUtils.EditingAnimation is null)
            {
                return;
            }

            var acomponent = CurrentPlayer.EntityReference.GetComponent<AnimationComponent>();
            var currentKeyFrame = acomponent.CurrentKeyFrame;
            // Current Keyframe Timing Text
            var framenumtext = acomponent.Animator.Keyframes[currentKeyFrame].Frames.ToString();
            _currentframetime.SetText(framenumtext);

            // Total Amount of Animation KeyFrames
            var totalframenumtext = acomponent.Animator.GetTotalFrames().ToString();
            _frametotal.SetText(totalframenumtext);

            LayerPanelUpdate(currentKeyFrame);

            UIAutoScaleTextTextPanel<string> listSelected = (UIAutoScaleTextTextPanel<string>)_testlist._items.FirstOrDefault(x => EditorUtils.EditingAnimation.Name.Equals((x as UIAutoScaleTextTextPanel<string>).Text));
            if (listSelected != null)
            {
                listSelected.TextColor = Color.Aqua;

                if (EditorUtils.AnimationEdited)
                    listSelected.TextColor = Color.OrangeRed;
            }

            if (EditorUtils.AnimationEdited)
                DisplayLayers(acomponent.Animation, currentKeyFrame);
        }

        internal void ButtonAction(Action<UIMouseEvent, UIElement> action,
            UIMouseEvent evt,
            UIElement listen,
            EditorMode[] modeRestriction = null,
            bool animationNeeded = false)
        {
            if (modeRestriction != null && modeRestriction.Length.Equals(0))
                return;

            if (modeRestriction == null || modeRestriction.Contains(HunterCombatMR.Instance.EditorInstance.CurrentEditMode) &&
                    (!animationNeeded || animationNeeded && EditorUtils.EditingAnimation != null))
            {
                action(evt, listen);
                Main.PlaySound(SoundID.MenuTick);
                var component = CurrentPlayer.EntityReference.GetComponent<AnimationComponent>();
                DisplayLayers(component.Animation, component.CurrentKeyFrame);
            }
        }

        private void CycleTexture(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!CurrentPlayer.EntityReference.HasComponent<AnimationComponent>())
            {
                var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
                if (layers.Count() == 1)
                {
                    AnimationBuilder anim = EditorUtils.EditingAnimation;
                    var layer = anim.Layers.FirstOrDefault(x => x.Name.Equals(layers.Single()));
                    // Turn this into either a dropdown list of tags or a textbox. Turning off for now
                }
            }
        }

        private void DisplayLayers(CustomAnimation animation,
            int keyFrame)
        {
            foreach (var layer in animation.GetOrderedActiveLayerData(keyFrame))
            {
                if (!_layerlist._items.Any(x => (x as LayerText).LayerRef.Equals(layer)))
                {
                    //_layerlist.Add(new LayerText(layer.Key, LayerTextInfo.None));
                }
            }
        }

        private void FrameTimeLogic(int amount)
        {
            if (CurrentPlayer == null || !CurrentPlayer.EntityReference.HasComponent<AnimationComponent>())
                return;

            var animator = CurrentPlayer.EntityReference.GetComponent<AnimationComponent>().Animator;
            if (!animator.Initialized)
                return;

            FrameIndex currentKeyframeIndex = animator.GetCurrentKeyframe();
            Keyframe keyFrame = animator.Keyframes[currentKeyframeIndex];

            if (amount + keyFrame.Frames <= 0)
                return;

            FrameLength newLength = amount + keyFrame.Frames;

            //EditorUtils.EditingAnimation.Layers[currentKeyframeIndex].Frames = newLength;
            //animator.Initialize(EditorUtils.EditingAnimation.Layers.FrameData);

            SetToClosestFrame(animator.Frame, animator.GetTotalFrames());
        }

        private static FrameIndex SetToClosestFrame(FrameIndex frame,
            int totalFrames)
        {
            if (frame > totalFrames)
            {
                frame--;
                SetToClosestFrame(frame, totalFrames);
            }
            return frame;
        }

        private void LayerPanelUpdate(int currentKeyFrame)
        {
            LayerBuilder layer = null;
            bool hasLayer = true;
            try
            {
                layer = EditorUtils.EditingAnimation.GetLayer(EditorUtils.HighlightedLayerNames.Single());
            }
            catch
            {
                hasLayer = false;
            }

            if (EditorUtils.HighlightedLayerNames.Count() != 1 || !hasLayer)
            {
                _currentlayertextureframe.SetText("");
                _layertexturebutton.SetText("");
                if (!_layerInfoPanel.IsCollapsed)
                    _layerInfoPanel.Collapse();
                return;
            }

            // Current Layer's Texture Frame
            string layertexframe = layer.GetLayerData(currentKeyFrame).SheetFrame.ToString() ?? "";
            _currentlayertextureframe.SetText(layertexframe);

            // Current Layer's Texture
            string layertexture = TextureUtils.GetTextureFromTag(new TextureTag(layer.Name, Point.Zero)).Name;
            _layertexturebutton.SetText(layertexture);

            if (_layerInfoPanel.LayerRef.Equals(layer))
            {
                _layerInfoPanel.LayerRef = layer;
            }

            if (_layerInfoPanel.IsCollapsed)
                _layerInfoPanel.Reveal();
        }

        private void LoadAnimationForPlayer(string name)
        {
            //var newAnim = ContentUtils.Get<ICustomAnimationV2>(name);
            //EditorUtils.EditingAnimation = newAnim;
            _animationname.Text = EditorUtils.EditingAnimation?.Name;
            _animationTimeline.SetAnimation(EditorUtils.EditingAnimation);
        }

        private void ModifyTextureFrame(int modifier)
        {
            if (!CurrentPlayer.EntityReference.HasComponent<AnimationComponent>())
            {
                return;
            }

            var layers = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
            if (layers.Count() != 1)
            {
                return;
            }
            /*
            var component = CurrentPlayer.EntityReference.GetComponent<AnimationComponent>();
            var key = component.Animator.GetCurrentKeyFrame();
            var layer = component.Animation.Layers[HunterCombatMR.Instance.EditorInstance.HighlightedLayers.Single(), key];

            var totalFrames = TextureUtils.GetTotalTextureFrames(TextureUtils.GetTextureFromTag(layer.Layer.Tag), layer.Layer.Tag);

            int newFrame = layer.FrameData.SheetFrame + modifier;

            if (newFrame >= totalFrames)
                newFrame = 0;

            if (newFrame < 0)
                newFrame = totalFrames - 1;

            layer.FrameData.SheetFrame = newFrame;*/
        }

        private void NextTextureFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            ModifyTextureFrame(1);
        }

        private void PreviousTextureFrame(UIMouseEvent evt, UIElement listeningElement)
        {
            ModifyTextureFrame(-1);
        }

        private void ReloadAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (loadTimer == 0 && EditorUtils.EditingAnimation != null)
            {
                LoadAnimationForPlayer(EditorUtils.EditingAnimation.Name);
                loadTimer++;
            }
        }

        private void SaveAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_animationname.Interacting || EditorUtils.EditingAnimation == null || saveTimer != 0)
                return;

            if (string.IsNullOrWhiteSpace(_animationname.Text))
            {
                _animationname.Text = EditorUtils.EditingAnimation.Name;
            }

            EditorUtils.EditingAnimation.Build();

            /*
            FileSaveStatus saveStatus = ContentUtils.SaveAnimation(EditorUtils.EditingAnimation);

            if (saveStatus != FileSaveStatus.Saved)
            {
                Main.NewText($"Animation could not save! Save Status: {saveStatus.ToString()}");
                return;
            }
            */
            saveTimer++;
            EditorUtils.AnimationEdited = false;
        }

        private void SelectAnimation(UIMouseEvent evt, UIElement listeningElement)
        {
            LoadAnimationForPlayer((listeningElement as UIAutoScaleTextTextPanel<string>).Text);
        }
    }
}