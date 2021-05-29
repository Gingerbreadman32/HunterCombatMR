using HunterCombatMR.Enumerations;
using HunterCombatMR.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces
{
    public interface ICustomAnimation
        : IHunterCombatContentInstance
    {
        [JsonIgnore]
        Animator AnimationData { get; }

        AnimationType AnimationType { get; }

        [JsonIgnore]
        bool IsInitialized { get; }

        bool IsStoredInternally { get; }
        ExtraAnimationData LayerData { get; }
        string DisplayName { get; }

        void AddKeyFrame(Keyframe duplicate);

        void AddKeyFrame(FrameLength frameLength, IDictionary<AnimationLayer, LayerFrameInfo> layerInfo = null);

        void Initialize();

        void MoveKeyFrame(int keyFrameIndex, int newFrameIndex);

        void RemoveKeyFrame(int keyFrameIndex);

        void Uninitialize();

        void Update();

        void UpdateLoopType(LoopStyle newLoopType);
    }
}