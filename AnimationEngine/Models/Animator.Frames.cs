using HunterCombatMR.AnimationEngine.Interfaces;
using System;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public partial class Animator<TAnimated, TObject, TObjectAnimated>
        : IAnimator
        where TAnimated : IAnimated
        where TObject : IAnimatedEntity<TObjectAnimated>
        where TObjectAnimated : IAnimated
    {
        public int TotalFrames { get => KeyFrames.Sum(x => x.Value.FrameLength); }

        public int FinalFrame { get => TotalFrames - 1; }

        public int CurrentKeyFrameProgress
        {
            get => CurrentFrame - GetCurrentKeyFrame().StartingFrameIndex;
        }

        public int CurrentKeyFrameIndex
        {
            get => KeyFrames.IndexOfValue(GetCurrentKeyFrame());
        }

        public void AdjustKeyFrameLength(FrameIndex keyFrameIndex,
                    FrameLength newFrameLength,
                    FrameLength defaultLength)
        {
            var newKeyframe = new KeyFrame(KeyFrames[keyFrameIndex]);
            newKeyframe.FrameLength = newFrameLength;

            KeyFrames.Remove(keyFrameIndex);
            KeyFrames.Add(keyFrameIndex, newKeyframe);

            SyncFrames(defaultLength);
        }

        public void SyncFrames(FrameLength defaultLength)
        {
            Uninitialize();

            Initialize(new KeyFrameProfile(KeyFrames, defaultLength), CurrentLoopStyle);
        }
    }
}