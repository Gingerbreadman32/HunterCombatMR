using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Animation.Entity;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Extensions
{
    public static class AnimationExtensions
    {
        public static IOrderedEnumerable<KeyValuePair<string, EntityAnimationLayerData>> GetOrderedActiveLayerData(this EntityAnimation animation,
            FrameIndex keyframe)
            => animation.Layers
                .Where(x => x.LayerData.Any(y => y.AnimationKeyframe.Equals(keyframe)))
                .Select(x => new KeyValuePair<string, EntityAnimationLayerData>(x.Name, x.LayerData.Single(y => y.AnimationKeyframe.Equals(keyframe))))
                .OrderByDescending(x => x.Value.Depth);
    }
}