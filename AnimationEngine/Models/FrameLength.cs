using HunterCombatMR.Models;

namespace HunterCombatMR.AnimationEngine.Models
{
    public sealed class FrameLength
        : PositiveInteger<FrameLength>
    {
        public FrameLength()
            : base(1, 1)
        {
        }

        public FrameLength(int length)
            : base(length, 1)
        {
        }
    }
}