using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class ActionLogicMethod<TEntity, TAnimationType> where TEntity
        : IAnimatedEntity<TAnimationType> where TAnimationType
        : Animation<TEntity, TAnimationType>
    {
        #region Public Methods

        public abstract IDictionary<string, string> ActionLogic(TEntity @object,
            int currentFrame,
            int currentKeyFrameTime,
            IDictionary<string, string> @params);

        #endregion Public Methods
    }
}