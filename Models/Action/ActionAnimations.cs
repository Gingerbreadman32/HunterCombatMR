using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Utilities;
using System;
using System.Linq;

namespace HunterCombatMR.Models.Action
{
    public sealed class ActionAnimations
        : KeyframeDataCollection<ICustomAnimationV2, AnimationData>
    {
        public ActionAnimations()
            : base()
        { }

        public ActionAnimations(ActionAnimations copy)
            : base(copy)
        { }

        public void AddAnimation(string animationName)
        {
            ICustomAnimationV2 animation;

            if (!ContentUtils.TryGet(animationName, out animation))
                throw new ArgumentOutOfRangeException($"Animation with name {animationName} does not exist within loaded animations!");

            Add(animation);
        }

        public void SwitchAnimations(FrameIndex firstAnimationIndex,
                    FrameIndex secondAnimationIndex)
        {
            if (!FrameData.ContainsKey(firstAnimationIndex))
                throw new ArgumentOutOfRangeException($"No animation exists at this keyframe: {firstAnimationIndex}!");

            if (!FrameData.ContainsKey(secondAnimationIndex))
                throw new ArgumentOutOfRangeException($"No animation exists at this keyframe: {secondAnimationIndex}!");

            var temp = FrameData[firstAnimationIndex];

            FrameData[firstAnimationIndex] = FrameData[secondAnimationIndex];
            FrameData[secondAnimationIndex] = temp;
        }
    }
}