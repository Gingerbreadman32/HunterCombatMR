using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface ICustomAction<TEntity, TAnimationType>
        : IHunterCombatContentInstance
        where TEntity : IAnimatedEntity<TAnimationType>
        where TAnimationType : Animation<TEntity, TAnimationType>
    {
        IEnumerable<TAnimationType> Animations { get; }
        IDictionary<string, string> DefaultParameters { get; set; }
        IEnumerable<KeyFrameEvent<TEntity, TAnimationType>> KeyFrameEvents { get; set; }
        KeyFrameProfile KeyFrameProfile { get; }
        string Name { get; }

        void AddKeyFrameEvent(ActionLogicMethod<TEntity, TAnimationType> actionLogicMethod);

        void Initialize();

        void Update(IAnimator animator);
    }
}