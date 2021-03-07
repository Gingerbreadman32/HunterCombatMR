using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;

namespace HunterCombatMR.AttackEngine.Models
{
    public class KeyFrameEvent<TObject, TAnimationType> where TObject
        : IAnimated<TAnimationType> where TAnimationType
        : Animation
    {
        #region Public Constructors

        public KeyFrameEvent(int tag,
            KeyFrame keyFrame,
            ActionLogicMethod<TObject, TAnimationType> actionLogic)
        {
            Tag = tag;
            EndKeyFrame = keyFrame;
            StartKeyFrame = keyFrame;
            ActionLogic = actionLogic;
        }

        public KeyFrameEvent(int tag,
            KeyFrame startKeyFrame,
            KeyFrame endKeyFrame,
            ActionLogicMethod<TObject, TAnimationType> actionLogic)
        {
            Tag = tag;
            EndKeyFrame = endKeyFrame;
            StartKeyFrame = startKeyFrame;
            ActionLogic = actionLogic;
        }

        #endregion Public Constructors

        #region Public Properties

        public ActionLogicMethod<TObject, TAnimationType> ActionLogic { get; }
        public KeyFrame EndKeyFrame { get; }
        public bool IsEnabled { get; set; } = true;
        public KeyFrame StartKeyFrame { get; }
        public int Tag { get; }

        #endregion Public Properties
    }
}