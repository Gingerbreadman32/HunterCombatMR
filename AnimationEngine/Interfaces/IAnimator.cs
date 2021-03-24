using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimator
    {
        int CurrentFrame { get; set; }
        int CurrentKeyFrameIndex { get; }
        int CurrentKeyFrameProgress { get; }
        LoopStyle CurrentLoopStyle { get; set; }
        int FinalFrame { get; }
        bool InProgress { get; set; }
        bool IsInitialized { get; set; }
        bool IsPlaying { get; set; }
        SortedList<int, KeyFrame> KeyFrames { get; }
        IDictionary<string, string> Parameters { get; set; }
        int TotalFrames { get; }

        void AdjustKeyFrameLength(FrameIndex keyFrameIndex, FrameLength newFrameLength, FrameLength defaultLength);
        void AdvanceFrame(int framesAdvancing = 1, bool bypassPause = false);
        void AdvanceToNextKeyFrame();
        void AppendKeyFrame(FrameLength keyFrameLength);
        bool CheckCurrentKeyFrame(int keyFrameIndex);
        void CreateKeyFrames(KeyFrameProfile keyFrameProfile, LoopStyle loopStyle = LoopStyle.Once);
        KeyFrame GetCurrentKeyFrame();
        void Initialize(KeyFrameProfile keyFrameProfile, LoopStyle loopStyle = LoopStyle.Once);
        void PauseAnimation();
        void ResetAnimation(bool startPlaying = true);
        void ReverseFrame(int framesReversing = 1);
        void ReverseToPreviousKeyFrame();
        void StartAnimation();
        void StopAnimation();
        void SyncFrames(FrameLength defaultLength);
        void Uninitialize();
    }
}