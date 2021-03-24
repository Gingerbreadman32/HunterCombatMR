using HunterCombatMR.Models;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class FrameIndex
        : PositiveInteger<FrameIndex>
    {
        #region Public Constructors

        public FrameIndex()
            : base()
        {
        }

        public FrameIndex(int index)
            : base(index)
        {
        }

        #endregion Public Constructors
    }
}