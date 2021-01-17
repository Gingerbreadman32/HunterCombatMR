using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    internal class UIEditorPopUpState
        : UIState
    {
        #region Private Fields

        private UIAutoScaleTextTextPanel<string> _modeswitch;
        private List<PopUpButton> PopUps;

        #endregion Private Fields

        #region Internal Constructors

        internal UIEditorPopUpState()
        {
            PopUps = new List<PopUpButton>();
        }

        #endregion Internal Constructors

        #region Internal Properties

        internal IEnumerable<LayerText> Layers { get; private set; }

        #endregion Internal Properties

        #region Public Methods

        public override void OnInitialize()
        {
            base.OnInitialize();

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
            _modeswitch.OnClick += (evt, list) => ModeSwitch(evt, list);

            Append(_modeswitch);
        }

        public void RemoveAllChildrenOfType<T>() where T : UIElement
        {
            var tempElements = new List<UIElement>(Elements);
            foreach (T element in Elements.Where(x => x.GetType().IsAssignableFrom(typeof(T))))
            {
                element.Parent = null;
                tempElements.Remove(element);
            }
            Elements = tempElements;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _modeswitch.SetText(HunterCombatMR.Instance.EditorInstance.CurrentEditMode.GetDescription());
        }

        #endregion Public Methods

        #region Internal Methods

        internal void UpdateActiveLayers(IEnumerable<LayerText> layers)
        {
            Layers = layers;
            var highlighted = HunterCombatMR.Instance.EditorInstance.HighlightedLayers;

            if (highlighted.Count() == 1 && layers.Any(x => x.Layer.Name.Equals(highlighted.FirstOrDefault())))
            {
                var selected = layers.FirstOrDefault(y => y.Layer.Name.Equals(highlighted.FirstOrDefault()));
                var currentKeyFrame = HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing.AnimationData.GetCurrentKeyFrameIndex();
                if (!PopUps.Any(x => x.AttachedElement == selected))
                {
                    ClearPopUps();

                    var upButton = new PopUpButton('\u25B2', 40f, 40f, selected, new Vector2(120f, -50f)).WithFadedMouseOver();
                    upButton.OnClick += (evt, list) => HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing.UpdateLayerDepth(-1, selected.Layer, layers.Select(x => x.Layer));
                    PopUps.Add(upButton);

                    var enableButton = new PopUpButton((selected.Layer.KeyFrames[currentKeyFrame].IsEnabled) ? '\u2713' : '\u2715', 40f, 40f, selected, new Vector2(120f, 0f)).WithFadedMouseOver();
                    enableButton.OnClick += (evt, list) => { HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing.UpdateLayerVisibility(selected.Layer); HunterCombatMR.Instance.EditorInstance.AnimationEdited = true; };
                    PopUps.Add(enableButton);

                    var downButton = new PopUpButton('\u25BC', 40f, 40f, selected, new Vector2(120f, 50f)).WithFadedMouseOver();
                    downButton.OnClick += (evt, list) => HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing.UpdateLayerDepth(1, selected.Layer, layers.Select(x => x.Layer));
                    PopUps.Add(downButton);

                    PopUps.ForEach(x => Append(x));
                }
            }
            else
            {
                ClearPopUps();
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void ClearPopUps()
        {
            RemoveAllChildrenOfType<PopUpButton>();
            PopUps.Clear();
        }

        private void ModeSwitch(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode))
            {
                HunterCombatMR.Instance.EditorInstance.CurrentEditMode = EditorMode.None;
            }
            else
            {
                HunterCombatMR.Instance.EditorInstance.CurrentEditMode = EditorMode.EditMode;
            }

            Main.PlaySound(SoundID.MenuTick);
        }

        #endregion Private Methods
    }
}