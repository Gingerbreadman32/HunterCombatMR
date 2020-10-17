using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Services
{
    public class KeyFrameManager
    {
        /// <summary>
        /// Essentially creates an animation by filling an empty animation with keyframes, using this will replace current animation information so be careful.
        /// </summary>
        /// <param name="animation">The animation to fill</param>
        /// <param name="keyFrameAmount">The amount of key frames you want the animation to be not exceeding the amount of sprites in the sheet</param>
        /// <param name="startingSpriteIndex">The sprite the animation will want to start at</param>
        /// <param name="averageKeyFrameSpeed">The average speed any keyframes not defined will linger for</param>
        /// <param name="keyFrameSpeeds">A dictionary determining how long each keyframe will linger for at which index</param>
        /// <param name="startPlaying">Whether or not to start playing the animation after.</param>
        public void FillAnimationKeyFrames(IAnimation animation, 
            int keyFrameAmount, 
            int averageKeyFrameSpeed, 
            IDictionary<int, int> keyFrameSpeeds = null,
            int startingSpriteIndex = 0,
            bool startPlaying = true)
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

                AppendKeyFrame(animation, startingSpriteIndex + k, frameSpeed);
            }

            animation.KeyFrames.Sort();
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
        public void FillAnimationKeyFrames(IAnimation animation,
            KeyFrameProfile frameProfile,
            bool startPlaying = true)
        {
            FillAnimationKeyFrames(animation, frameProfile.KeyFrameAmount, frameProfile.DefaultKeyFrameSpeed, frameProfile.SpecificKeyFrameSpeeds, frameProfile.StartingSpriteIndex, startPlaying);
        }

        public void AppendKeyFrame(IAnimation animation,
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

        public void AppendKeyFrame(IAnimation animation,
            int spriteIndex, 
            int keyFrameLength)
        {
            KeyFrame newKeyFrame = new KeyFrame(spriteIndex, keyFrameLength);
            AppendKeyFrame(animation, newKeyFrame);
        }

        public void AppendKeyFrames(IAnimation animation,
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
        public IEnumerable<int> GetFramesKeyFrameIsActive(IAnimation animation,
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

        public void SyncFrames(IAnimation animation)
        {
            var keyFrameActiveFrames = new Dictionary<KeyFrame, IEnumerable<int>>();

            foreach (var keyframe in animation.KeyFrames)
            {
                keyFrameActiveFrames.Add(keyframe, GetFramesKeyFrameIsActive(keyframe));
            }

            foreach (var keyframe in keyFrameActiveFrames)
            {
            }
        }

        public void AdjustKeyFrameLength(int keyFrameIndex, int newFrameLength)
        {
        }

        /// <summary>
        /// Adjusts the entirety of the animation by the factor given
        /// </summary>
        /// <param name="speedFactor">The factor in which the frame timings will be multiplied</param>
        /// <param name="adjustFaster">True will reduce the amount of frames keyframes will last, false will increase them. (True for faster, false for slower)</param>
        /// <param name="roundUp">Whether or not to round up the new values, false will round them down instead</param>
        public void AdjustAnimationSpeed(int speedFactor, bool adjustFaster, bool roundUp = false)
        {
        }
    }
}