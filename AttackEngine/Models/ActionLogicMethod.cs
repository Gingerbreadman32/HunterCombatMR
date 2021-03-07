using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class ActionLogicMethod<TObject, TAnimationType> where TObject
        : IAnimated<TAnimationType> where TAnimationType
        : Animation
    {
        #region Public Methods

        public abstract IDictionary<string, string> ActionLogic(TObject @object,
            int currentFrame,
            int currentKeyFrameTime,
            IDictionary<string, string> @params);

        #endregion Public Methods
    }
}