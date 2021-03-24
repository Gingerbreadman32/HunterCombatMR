using HunterCombatMR.AnimationEngine.Models;

namespace HunterCombatMR.Extensions
{
    public static class IntExtentions
    {
        #region Public Methods

        public static FrameIndex ToFIndex(this int val)
            => new FrameIndex(val);

        public static FrameLength ToFLength(this int val)
                    => new FrameLength(val);

        #endregion Public Methods
    }
}