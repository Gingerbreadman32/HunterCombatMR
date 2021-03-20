using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct EventTag
    {
        #region Private Fields

        private KeyValuePair<int, int> _frameRange;
        private int _tagRef;

        #endregion Private Fields

        #region Public Constructors

        public EventTag(int tag,
            int startFrame,
            int endFrame)
        {
            _tagRef = tag;
            _frameRange = new KeyValuePair<int, int>(startFrame, endFrame);
        }

        #endregion Public Constructors

        #region Public Properties

        public int TagReference { get => _tagRef; }

        #endregion Public Properties

        #region Public Methods

        public int GetEndFrame()
            => _frameRange.Value;

        public int GetStartFrame()
            => _frameRange.Key;

        public bool CheckFrameBetween(int frame)
            => (frame <= GetEndFrame()) && (frame >= GetStartFrame());

        #endregion Public Methods
    }
}