using HunterCombatMR.Models;
using Newtonsoft.Json;

namespace HunterCombatMR.Models.Action
{
    public struct EventTag
    {
        [JsonConstructor]
        public EventTag(int tag,
            FrameIndex startKeyframe,
            FrameIndex endKeyframe)
        {
            ID = tag;
            StartKeyframe = startKeyframe;
            EndKeyframe = endKeyframe;
            Lifetime = false;
        }

        public EventTag(int tag,
            FrameIndex startKeyframe,
            int eventLength)
        {
            ID = tag;
            StartKeyframe = startKeyframe;
            EndKeyframe = startKeyframe + (eventLength - 1);
            Lifetime = false;
        }

        public EventTag(int tag)
        {
            ID = tag;
            Lifetime = true;
            StartKeyframe = 0;
            EndKeyframe = 0;
        }

        public FrameIndex EndKeyframe { get; }

        public int ID { get; }

        public bool Lifetime { get; }

        public FrameIndex StartKeyframe { get; }

        public bool IsActive(int frame)
            => frame <= EndKeyframe && frame >= StartKeyframe || Lifetime;
    }
}