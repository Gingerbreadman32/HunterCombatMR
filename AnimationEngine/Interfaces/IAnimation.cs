using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AnimationEngine.Services;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimation
    {
        /// <summary>
        /// The list of keyframes and their timings
        /// </summary>
        List<KeyFrame> KeyFrames { get; }

        /// <summary>
        /// The total amount of frames the animation lasts
        /// </summary>
        int TotalFrames { get; }

        /// <summary>
        /// The current frame
        /// </summary>
        int CurrentFrame { get; }

        /// <summary>
        /// Whether or not the animation is currently being played while part of an active updating object.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Whether or not the animation has been initialized by a keyframe manager
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initialize the animation, only <seealso cref="KeyFrameManager"/> should be calling this
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the current keyframe index
        /// </summary>
        /// <returns>A <see cref="KeyFrame"/> object</returns>
        KeyFrame GetCurrentKeyFrame();

        /// <summary>
        /// Checks to see if the current keyframe is equal to the one provided
        /// </summary>
        /// <param name="keyFrameIndex">Keyframe to query</param>
        /// <returns>True if the current keyframe equals the one provided</returns>
        bool CheckCurrentKeyFrame(int keyFrameIndex);

        /// <summary>
        /// Checks to see if the current keyframe's lifespan equals the passed in frame time (Ex. First frame the keyframe exists is 0)
        /// </summary>
        /// <param name="relativeFrame">The frame time to check against</param>
        /// <returns>True if the current keyframe is at the current relative frame</returns>
        bool CheckCurrentKeyFrameProgress(int relativeFrame);

        /// <summary>
        /// Returns the current frame relative to when the current keyframe was started
        /// </summary>
        /// <returns>How long the keyframe has been active</returns>
        /// <remarks>
        /// Useful for getting the start of the keyframe
        /// </remarks>
        int GetCurrentKeyFrameProgress();

        /// <summary>
        /// Advances the current animations frames by the specified amount
        /// </summary>
        /// <param name="framesAdvancing">The amount of frames to advance</param>
        /// <param name="bypassPause">Wether or not to adhere to the isPlaying datapoint</param>
        void AdvanceFrame(int framesAdvancing = 1, bool bypassPause = false);

        /// <summary>
        /// Go back a number of frames equal to the amount passed
        /// </summary>
        /// <param name="framesReversing">The amount of frames to reverse</param>
        /// <param name="bypassPause">Wether or not to adhere to the isPlaying datapoint</param>
        void ReverseFrame(int framesReversing = 1, bool bypassPause = false);

        /// <summary>
        /// Set the animation to a specific frame
        /// </summary>
        /// <param name="newFrame">The frame to set the animation to</param>
        void SetCurrentFrame(int newFrame);

        /// <summary>
        /// Adds frames to the total count, don't use this without explicit knowledge of what you're doing.
        /// </summary>
        /// <param name="frameAmount">Amount of frames to add to the total.</param>
        void AddFrames(int frameAmount);

        /// <summary>
        /// Returns the total keyframes
        /// </summary>
        /// <returns>The total amount of keyframes</returns>
        int GetTotalKeyFrames();

        /// <summary>
        /// Allow update to start animating the animation
        /// </summary>
        void StartAnimation();

        /// <summary>
        /// Stop update from animating the animation at the current frame
        /// </summary>
        void StopAnimation();

        /// <summary>
        /// Reset the keyframes to an empty list
        /// </summary>
        void ResetKeyFrames();

        /// <summary>
        /// Sets the frame back to 0
        /// </summary>
        /// <param name="startPlaying">Whether to play the animation right after resetting</param>
        void ResetAnimation(bool startPlaying = true);

        /// <summary>
        /// Gets the actual index of the final frame
        /// </summary>
        /// <returns>Literally just the total frames minus one</returns>
        /// <remarks>
        /// Probably unecessary but it looks cleaner than doing this every time it's used
        /// </remarks>
        int GetFinalFrame();
    }
}