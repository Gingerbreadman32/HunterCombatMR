using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct EventTagInfo
    {
        private KeyValuePair<int, int> _frameRange;

        public EventTagInfo(int tag,
            int startFrame,
            int endFrame)
        {
            TagReference = tag;
            _frameRange = new KeyValuePair<int, int>(startFrame, endFrame);
        }

        public int EndFrame
        {
            get => _frameRange.Value;
        }

        public int StartFrame
        {
            get => _frameRange.Key;
        }

        public int TagReference { get; }

        public bool IsActive(int frame)
            => (frame <= EndFrame) && (frame >= StartFrame);
    }
}