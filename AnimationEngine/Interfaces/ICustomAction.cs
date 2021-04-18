using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface ICustomAction<THolder, TEntity>
        : IAnimated,
        INamed where THolder
        : IEntityHolder<TEntity> where TEntity
        : Entity
    {
        SortedList<int, IAnimation> Animations { get; }
        IDictionary<string, string> DefaultParameters { get; set; }
        IEnumerable<KeyFrameEvent<THolder, TEntity>> KeyFrameEvents { get; set; }

        void AddKeyFrameEvent(ActionLogicMethod<THolder, TEntity> actionLogicMethod);

        void Initialize();
    }
}