using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Interfaces;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class ActionLogicMethod<THolder, TEntity> where THolder
        : IEntityHolder<TEntity> where TEntity
        : Entity

    {
        public virtual FrameLength DefaultLength { get; } = new FrameLength();

        public abstract IDictionary<string, string> ActionLogic(THolder @object,
            int currentFrame,
            int currentKeyFrameTime,
            IDictionary<string, string> @params);
    }
}