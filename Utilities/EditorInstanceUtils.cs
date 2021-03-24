using HunterCombatMR.AnimationEngine.Models;

namespace HunterCombatMR.Utilities
{
    public static class EditorInstanceUtils
    {
        #region Public Methods

        public static PlayerActionAnimation EditingAnimation
        {
            get => HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing;
            set => HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing = value;
        }

        #endregion Public Methods
    }
}