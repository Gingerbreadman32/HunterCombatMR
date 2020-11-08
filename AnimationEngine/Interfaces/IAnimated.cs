using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimated
    {
        string Name { get; }

        IAnimation Animation { get; }

        void Initialize();

        void Play();

        void Stop();

        void Pause();

        void Restart();

        void Update();

        void UpdateKeyFrameLength(int keyFrameIndex, int frameAmount, bool setAmount = false, bool setDefault = false);

        void UpdateLoopType(LoopStyle newLoopType);
    }
}