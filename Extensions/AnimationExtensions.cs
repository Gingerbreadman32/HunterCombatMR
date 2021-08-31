using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Extensions
{
    public static class AnimationExtensions
    {
        public static IOrderedEnumerable<KeyValuePair<string, LayerData>> GetOrderedActiveLayerData(this CustomAnimation animation,
            FrameIndex keyframe)
            => animation.Layers
                .Where(x => x.LayerData.Any(y => y.AnimationKeyframe.Equals(keyframe)))
                .Select(x => new KeyValuePair<string, LayerData>(x.Name, x.LayerData.Single(y => y.AnimationKeyframe.Equals(keyframe))))
                .OrderByDescending(x => x.Value.Depth);
    }
}