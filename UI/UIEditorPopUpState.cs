using HunterCombatMR.AnimationEngine.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    internal class UIEditorPopUpState
        : UIState
    {
        private List<PopUpButton> PopUps;

        internal IEnumerable<LayerText> Layers { get; private set; }

        internal UIEditorPopUpState()
        {
            PopUps = new List<PopUpButton>();
        }

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

                    var upButton = new PopUpButton('\u25B2', 40f, 40f, selected, new Vector2(160f, -50f)).WithFadedMouseOver();
                    upButton.OnClick += (evt, list) => HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing.UpdateLayerDepth(-1, selected.Layer, layers.Select(x => x.Layer));
                    PopUps.Add(upButton);

                    var enableButton = new PopUpButton((selected.Layer.KeyFrames[currentKeyFrame].IsEnabled) ? '\u2713' : '\u2715', 40f, 40f, selected, new Vector2(160f, 0f)).WithFadedMouseOver();
                    enableButton.OnClick += (evt, list) => { HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing.UpdateLayerVisibility(selected.Layer); HunterCombatMR.Instance.EditorInstance.AnimationEdited = true; };
                    PopUps.Add(enableButton);

                    var downButton = new PopUpButton('\u25BC', 40f, 40f, selected, new Vector2(160f, 50f)).WithFadedMouseOver();
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

        private void ClearPopUps()
        {
            RemoveAllChildren();
            PopUps.Clear();
        }
    }
}