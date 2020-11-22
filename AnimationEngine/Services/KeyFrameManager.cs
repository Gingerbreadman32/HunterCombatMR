using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Services
{
    public class KeyFrameManager
    {
        /// <summary>
        /// Essentially creates an animation by filling an empty animation with keyframes, using this will replace current animation information so be careful.
        /// </summary>
        /// <param name="animation">The animation to fill</param>
        /// <param name="keyFrameAmount">The amount of key frames you want the animation to be not exceeding the amount of sprites in the sheet</param>
        /// <param name="averageKeyFrameSpeed">The average speed any keyframes not defined will linger for</param>
        /// <param name="keyFrameSpeeds">A dictionary determining how long each keyframe will linger for at which index</param>
        /// <param name="startPlaying">Whether or not to start playing the animation after.</param>
        public void FillAnimationKeyFrames(ref AnimatedData animation, 
            int keyFrameAmount, 
            int averageKeyFrameSpeed, 
            IDictionary<int, int> keyFrameSpeeds = null,
            bool startPlaying = true,
            LoopStyle loopStyle = LoopStyle.Once)
        {
            animation.StopAnimation();
            animation.SetCurrentFrame(0);
            animation.ResetKeyFrames();

            for (var k = 0; k < keyFrameAmount; k++)
            {
                int frameSpeed;
                if (keyFrameSpeeds != null && keyFrameSpeeds.ContainsKey(k))
                    frameSpeed = keyFrameSpeeds[k];
                else
                    frameSpeed = averageKeyFrameSpeed;

                AppendKeyFrame(animation, frameSpeed);
            }

            animation.KeyFrames.Sort();
            animation.SetLoopMode(loopStyle);
            animation.Initialize();

            if (startPlaying)
                animation.StartAnimation();
        }

        /// <summary>
        /// Overloaded FillAnimationKeyFrames that takes the object which stores most of this data
        /// </summary>
        /// <param name="animation">The animation to fill</param>
        /// <param name="frameProfile">A <seealso cref="KeyFrameProfile"/></param>
        /// <param name="startPlaying">Whether or not to start playing the animation after.</param>
        public void FillAnimationKeyFrames(AnimatedData animation,
            KeyFrameProfile frameProfile,
            bool startPlaying = true,
            LoopStyle loopStyle = LoopStyle.Once)
        {
            FillAnimationKeyFrames(ref animation, frameProfile.KeyFrameAmount, frameProfile.DefaultKeyFrameSpeed, frameProfile.SpecificKeyFrameSpeeds, startPlaying, loopStyle);
        }

        public void AppendKeyFrame(AnimatedData animation,
            KeyFrame newKeyFrame)
        {
            if (newKeyFrame.FrameLength > 0)
            {
                newKeyFrame.StartingFrameIndex = animation.TotalFrames;
                animation.AddFrames(newKeyFrame.FrameLength);
                animation.KeyFrames.Add(newKeyFrame);
            }
            else
            {
                throw new Exception("Keyframe must be longer than 0 frames!");
            }
        }

        public void AppendKeyFrame(AnimatedData animation,
            int keyFrameLength)
        {
            KeyFrame newKeyFrame = new KeyFrame(animation.TotalFrames, keyFrameLength, animation.GetTotalKeyFrames());
            AppendKeyFrame(animation, newKeyFrame);
        }

        public void AppendKeyFrames(AnimatedData animation,
            IEnumerable<KeyFrame> newKeyFrames)
        {
            foreach (var keyframe in newKeyFrames)
            {
                AppendKeyFrame(animation, keyframe);
            }
        }

        /// <summary>
        /// Helper method to return all of the exact frames that a keyframe is active during an animation
        /// </summary>
        /// <param name="keyFrameIndex">The keyframe you want to check</param>
        /// <returns>A list of ints representing the frame indexes the keyframe is active</returns>
        public IEnumerable<int> GetFramesKeyFrameIsActive(AnimatedData animation,
            int keyFrameIndex)
            => GetFramesKeyFrameIsActive(animation.KeyFrames[keyFrameIndex]);

        /// <summary>
        /// Helper method to return all of the exact frames that a keyframe is active during an animation, overload for the keyframe object
        /// </summary>
        /// <param name="keyFrame">The keyframe you want to check</param>
        /// <returns>A list of ints representing the frame indexes the keyframe is active</returns>
        public IEnumerable<int> GetFramesKeyFrameIsActive(KeyFrame keyFrame)
        {
            var framesActive = new List<int>();

            for (var f = keyFrame.StartingFrameIndex; f <= keyFrame.GetFinalFrameIndex(); f++)
            {
                if (keyFrame.IsKeyFrameActive(f))
                    framesActive.Add(f);
                else
                    throw new Exception("Keyframe formatted incorrectly: Not active within set frames!");
            }

            return framesActive;
        }

        public void SyncFrames(AnimatedData animation)
        {
            animation.Uninitialize();
            var keyFrameActiveFrames = new Dictionary<int, int>();

            foreach (var keyframe in animation.KeyFrames.OrderBy(x => x.KeyFrameOrder))
            {
                keyFrameActiveFrames.Add(keyframe.KeyFrameOrder, keyframe.FrameLength);
            }

            FillAnimationKeyFrames(ref animation, keyFrameActiveFrames.Count, 0, keyFrameActiveFrames, false, animation.LoopMode);
        }

        public void AdjustKeyFrameLength(AnimatedData animation,
            int keyFrameIndex, 
            int newFrameLength,
            bool addToLength = false)
        {
            var newKeyframe = new KeyFrame(animation.KeyFrames[keyFrameIndex]);

            if (addToLength && newKeyframe.FrameLength + newFrameLength > 0)
                newKeyframe.FrameLength += newFrameLength;
            else if (!addToLength && newFrameLength > 0)
                newKeyframe.FrameLength = newFrameLength;
            else
                throw new Exception("Invalid frame time: must be more than 0.");

            animation.KeyFrames.Remove(animation.KeyFrames[keyFrameIndex]);
            animation.KeyFrames.Add(newKeyframe);

            SyncFrames(animation);
        }
    }
}