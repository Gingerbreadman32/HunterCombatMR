using HunterCombatMR.Models;
using System;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class FrameLength
        : PositiveInteger<FrameLength>
    {
        #region Public Constructors

        public FrameLength()
            : base(1, 1)
        {
        }

        public FrameLength(int length)
            : base(length, 1)
        {
        }

        #endregion Public Constructors
    }
}