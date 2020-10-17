using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class KeyFrameProfile
    {
        public int KeyFrameAmount { get; set; }
        public int DefaultKeyFrameSpeed { get; set; }
        public IDictionary<int, int> SpecificKeyFrameSpeeds { get; set; }
        public int StartingSpriteIndex { get; set; }

        public KeyFrameProfile(int keyFrameAmount,
            int defaultKeyFrameSpeed,
            IDictionary<int, int> keyFrameSpeeds = null,
            int startingSpriteIndex = 0)
        {
            KeyFrameAmount = keyFrameAmount;
            DefaultKeyFrameSpeed = defaultKeyFrameSpeed;
            StartingSpriteIndex = startingSpriteIndex;

            if (keyFrameSpeeds != null)
                SpecificKeyFrameSpeeds = new Dictionary<int, int>(keyFrameSpeeds);
            else
                SpecificKeyFrameSpeeds = new Dictionary<int, int>();
        }
    }
}