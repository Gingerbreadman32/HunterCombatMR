using HunterCombatMR.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces
{
    public interface IAnimatedEntity
    {
        SortedList<FrameIndex, IKeyframeData> AnimatorKeyFrameData { get; }
    }
}