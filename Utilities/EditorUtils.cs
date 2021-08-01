using HunterCombatMR.Interfaces.Animation;
using System.Collections.Generic;

namespace HunterCombatMR.Utilities
{
    public static class EditorUtils
    {
        public static bool AnimationEdited
        {
            get => HunterCombatMR.Instance.EditorInstance.AnimationEdited;
            set => HunterCombatMR.Instance.EditorInstance.AnimationEdited = value;
        }

        public static ICustomAnimationV2 EditingAnimation
        {
            get => HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing;
            set => HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing = value;
        }

        public static ICollection<string> HighlightedLayerNames
        {
            get => HunterCombatMR.Instance.EditorInstance.HighlightedLayers;
        }
    }
}