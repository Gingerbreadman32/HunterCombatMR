using HunterCombatMR.Models;
using System;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class FrameLength
        : PositiveInteger<FrameLength>
    {
        #region Public Constructors

        public FrameLength()
            : base(1)
        {
        }

        public FrameLength(int length)
            : base(length)
        {
            if (length == 0)
                throw new ArgumentOutOfRangeException("Frame length must be more than 0!");
        }

        #endregion Public Constructors
    }
}