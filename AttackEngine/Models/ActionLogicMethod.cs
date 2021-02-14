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

        public virtual void ActionLogic(TObject @object, int currentFrame, KeyFrame keyFrame, IDictionary<string, string> @params)
        {
            if (currentFrame.Equals(keyFrame.StartingFrameIndex))
                StartLogic(@object, @params);
            else if (currentFrame.Equals(keyFrame.GetFinalFrameIndex()))
                EndLogic(@object, @params);
            else
                UpdateLogic(@object, (keyFrame.StartingFrameIndex - currentFrame), @params);
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract void EndLogic(TObject @object, IDictionary<string, string> @params);

        protected abstract void StartLogic(TObject @object, IDictionary<string, string> @params);

        protected abstract void UpdateLogic(TObject @object, int keyFrameTime, IDictionary<string, string> @params);

        #endregion Protected Methods
    }
}