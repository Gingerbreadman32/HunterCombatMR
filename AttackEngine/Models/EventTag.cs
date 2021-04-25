using HunterCombatMR.AnimationEngine.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct EventTag
    {
        private KeyValuePair<FrameIndex, FrameIndex> _frameRange;

        [JsonConstructor]
        public EventTag(int tag,
            FrameIndex startFrame,
            FrameIndex endFrame)
        {
            Id = tag;
            _frameRange = new KeyValuePair<FrameIndex, FrameIndex>(startFrame, endFrame);
        }

        public FrameIndex EndKeyFrame
        {
            get => _frameRange.Value;
            set => _frameRange = new KeyValuePair<FrameIndex, FrameIndex>(StartKeyFrame, value);
        }

        public int Id { get; }

        public FrameIndex StartKeyFrame
        {
            get => _frameRange.Key;
            set => _frameRange = new KeyValuePair<FrameIndex, FrameIndex>(value, EndKeyFrame);
        }

        public bool IsActive(int frame)
            => (frame <= EndKeyFrame) && (frame >= StartKeyFrame);
    }
}