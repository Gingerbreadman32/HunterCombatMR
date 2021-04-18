using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct EventTagInfo
    {
        #region Private Fields

        private KeyValuePair<int, int> _frameRange;

        #endregion Private Fields

        #region Public Constructors

        public EventTagInfo(int tag,
            int startFrame,
            int endFrame)
        {
            TagReference = tag;
            _frameRange = new KeyValuePair<int, int>(startFrame, endFrame);
        }

        #endregion Public Constructors

        #region Public Properties

        public int TagReference { get; }

        public int EndFrame
        {
            get => _frameRange.Value;
        }

        public int StartFrame
        {
            get => _frameRange.Key;
        }

        #endregion Public Properties

        #region Public Methods

        public bool IsActive(int frame)
            => (frame <= EndFrame) && (frame >= StartFrame);

        #endregion Public Methods
    }
}