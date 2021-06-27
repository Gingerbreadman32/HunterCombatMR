using HunterCombatMR.Enumerations;
using HunterCombatMR.Models.Animation;

namespace HunterCombatMR.Interfaces.Animation
{
    public interface ICustomAnimationV2
        : IContent,
        IKeyframeDataReference
    {
        AnimationType AnimationType { get; }
        LoopStyle DefaultLoopStyle { get; set; }
        AnimationLayers Layers { get; }
    }
}