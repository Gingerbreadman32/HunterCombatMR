using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    public struct KeyFrameProfile
    {
        public int KeyFrameAmount { get; set; }
        public int DefaultKeyFrameSpeed { get; set; }
        public IDictionary<int, int> SpecificKeyFrameSpeeds { get; set; }

        public KeyFrameProfile(int keyFrameAmount,
            int defaultKeyFrameSpeed,
            IDictionary<int, int> keyFrameSpeeds = null)
        {
            KeyFrameAmount = keyFrameAmount;
            DefaultKeyFrameSpeed = defaultKeyFrameSpeed;

            if (keyFrameSpeeds != null)
                SpecificKeyFrameSpeeds = new Dictionary<int, int>(keyFrameSpeeds);
            else
                SpecificKeyFrameSpeeds = new Dictionary<int, int>();
        }
    }
}