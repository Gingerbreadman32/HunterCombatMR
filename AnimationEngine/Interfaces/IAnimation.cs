using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimation
        : IHunterCombatContentInstance,
        IAnimated
    {
        AnimationType AnimationType { get; }
        bool IsStoredInternally { get; }
        LayerData LayerData { get; }
        string Name { get; }
        [JsonIgnore]
        IAnimator AnimationData { get; }

        void Initialize();
        void AddKeyFrame(KeyFrame duplicate);
        void AddKeyFrame(FrameLength frameLength, IDictionary<AnimationLayer, LayerFrameInfo> layerInfo = null);
        void MoveKeyFrame(int keyFrameIndex, int newFrameIndex);
        void RemoveKeyFrame(int keyFrameIndex);
        void Play();
        void Stop();
        void Pause();
        void UpdateLoopType(LoopStyle newLoopType);
    }
}