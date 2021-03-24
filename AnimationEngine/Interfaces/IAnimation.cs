using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimation
        : IHunterCombatContentInstance
    {
        AnimationType AnimationType { get; }
        IDictionary<string, string> DefaultParameters { get; }
        bool IsInternal { get; }
        KeyFrameProfile KeyFrameProfile { get; }
        LayerData LayerData { get; }
        string Name { get; }
        [JsonIgnore]
        IAnimator AnimationData { get; }

        void Initialize();

        AnimationLayer GetLayer(string layerName);
        void AddKeyFrame(KeyFrame duplicate);
        void AddKeyFrame(FrameLength frameLength, IDictionary<AnimationLayer, LayerFrameInfo> layerInfo = null);
        void Draw();
        void DrawEffects();
        void MoveKeyFrame(int keyFrameIndex, int newFrameIndex);
        void RemoveKeyFrame(int keyFrameIndex);
        void Play();
        void Stop();
        void Restart();
        void Pause();
        void UpdateLayerDepth(int amount, AnimationLayer layerToMove, IEnumerable<AnimationLayer> layers);
        void UpdateLoopType(LoopStyle newLoopType);
        void UpdateLayerTexture(AnimationLayer layer, Texture2D texture);
        void UpdateLayerTextureFrame(AnimationLayer layer, int textureFrame);
    }
}