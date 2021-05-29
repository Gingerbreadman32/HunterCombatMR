using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Action;
using System.Collections.Generic;

namespace HunterCombatMR.Interfaces.Action
{
    public interface ICustomAction<T>
        : IHunterCombatContentInstance
    {
        ActionAnimations Animations { get; }
        string DisplayName { get; }
        ActionEvents<T> Events { get; }
        SortedList<FrameIndex, IKeyFrameData> FrameData { get; }

        void ActionLogic(T entity, Animator animator);
    }
}