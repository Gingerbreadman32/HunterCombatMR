﻿using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Action
{
    public sealed class ActionAnimations
        : KeyframeDataCollection<ICustomAnimationV2, AnimationData>
    {
        public ActionAnimations(string animationName)
            : base()
        {
            AddAnimation(animationName, 0);
        }

        public ActionAnimations(ActionAnimations copy)
            : base(copy)
        { }

        public ActionAnimations(IEnumerable<string> references, KeyframeData<AnimationData>[] frameData)
        {
            _references = new List<ICustomAnimationV2>();

            foreach (var name in references)
            {
                AddAnimation(name);
            }

            _frameData = frameData;
        }

        public void AddAnimation(string animationName)
        {
            ICustomAnimationV2 animation;

            if (!ContentUtils.TryGet(animationName, out animation))
                throw new ArgumentOutOfRangeException($"Animation with name {animationName} does not exist within loaded animations!");

            Add(animation);
            this[_frameData.Length] = new KeyframeData<AnimationData>(animation.TotalFrames, animationName, new AnimationData(animationName));
        }

        public void AddAnimation(string animationName,
            FrameIndex setFrame)
        {
            ICustomAnimationV2 animation;

            if (!ContentUtils.TryGet(animationName, out animation))
                throw new ArgumentOutOfRangeException($"Animation with name {animationName} does not exist within loaded animations!");

            Add(animation);
            AddToKeyframe(setFrame, animationName, new AnimationData(animationName));
        }

        public void SwitchAnimations(FrameIndex firstAnimationIndex,
                    FrameIndex secondAnimationIndex)
        {
            if (FrameData.Length < firstAnimationIndex)
                throw new ArgumentOutOfRangeException($"No animation exists at this keyframe: {firstAnimationIndex}!");

            if (FrameData.Length < secondAnimationIndex)
                throw new ArgumentOutOfRangeException($"No animation exists at this keyframe: {secondAnimationIndex}!");

            var temp = FrameData[firstAnimationIndex];

            FrameData[firstAnimationIndex] = FrameData[secondAnimationIndex];
            FrameData[secondAnimationIndex] = temp;
        }
    }
}