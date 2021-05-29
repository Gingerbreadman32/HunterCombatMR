using HunterCombatMR.Models;
using HunterCombatMR.Models.Animation;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.Extensions
{
    public static class FrameDataExtensions
    {
        /*
        public static KeyFrameData<LayerData> RemoveKeyframe(this KeyFrameData<LayerData> frameData,
            FrameIndex keyframe)
        {
            if (!frameData.Value.ContainsKey(keyframe))
                throw new ArgumentOutOfRangeException($"Keyframe index {keyframe} does not exist!");

            if (frameData.Value.Count <= 1)
                throw new ArgumentException("List of keyframes must include at least 1 keyframe, cannot return an empty list.");

            return new KeyFrameData<LayerData>(Dropkeyframe(frameData.Value, keyframe));
        }

        public static KeyFrameData SwitchKeyFrames(this KeyFrameData frameData,
                    FrameIndex leftKeyframe,
                    FrameIndex rightKeyframe)
        {
            if (!frameData.Value.ContainsKey(leftKeyframe))
                throw new ArgumentOutOfRangeException($"Keyframe index {leftKeyframe} does not exist!");

            if (!frameData.Value.ContainsKey(rightKeyframe))
                throw new ArgumentOutOfRangeException($"Keyframe index {rightKeyframe} does not exist!");

            var keyframes = frameData.Value;
            var left = keyframes[leftKeyframe];
            var right = keyframes[rightKeyframe];

            keyframes[rightKeyframe] = left;
            keyframes[leftKeyframe] = right;

            return new FrameData(keyframes);
        }

        private static SortedList<FrameIndex, FrameLength> Dropkeyframe(SortedList<FrameIndex, FrameLength> keyframes,
            FrameIndex keyframe)
        {
            keyframes.Remove(keyframe);
            int nextFrame = keyframe + 1;
            if (keyframes.ContainsKey(nextFrame))
            {
                keyframes.Add(keyframe, keyframes[nextFrame]);
                keyframes = Dropkeyframe(keyframes, nextFrame);
            }
            return keyframes;
        }
        */
    }
}