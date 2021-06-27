using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Animation;
using Newtonsoft.Json;
using System.Linq;

namespace HunterCombatMR.Models.Animation
{
    public class CustomAnimationV2
        : Content,
        ICustomAnimationV2
    {
        [JsonConstructor]
        public CustomAnimationV2(string internalName,
            AnimationType type,
            AnimationLayers layers)
            : base(internalName)
        {
            AnimationType = type;
            Layers = new AnimationLayers(layers);
        }

        public CustomAnimationV2(ICustomAnimation legacyAnimation)
            : base(legacyAnimation.InternalName)
        {
            AnimationType = legacyAnimation.AnimationType;
            DefaultLoopStyle = legacyAnimation.LayerData.Loop;
            Layers = new AnimationLayers(legacyAnimation.LayerData);
        }

        public CustomAnimationV2(string name,
            ICustomAnimationV2 copy)
            : base(name)
        {
            Layers = new AnimationLayers(copy.Layers);
            AnimationType = copy.AnimationType;
            DefaultLoopStyle = copy.DefaultLoopStyle;
        }

        public AnimationType AnimationType { get; }

        public LoopStyle DefaultLoopStyle { get; set; }
        public AnimationLayers Layers { get; set; }

        [JsonIgnore]
        public string ReferenceName => InternalName;

        public int TotalFrames { get => Layers.FrameData.Values.Sum(x => x.Frames); }

        public override IContent CreateNew(string internalName)
            => new CustomAnimationV2(internalName, this);
    }
}