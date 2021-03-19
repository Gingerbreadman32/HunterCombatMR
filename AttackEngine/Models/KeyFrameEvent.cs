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
            KeyFrame = keyFrame;
            ActionLogic = actionLogic;
            EndKeyFrame = new KeyFrame(keyFrame.GetFinalFrameIndex() + 1, 1, keyFrame.KeyFrameOrder + 1);
        }

        #endregion Public Constructors

        #region Public Properties

        public ActionLogicMethod<TObject, TAnimationType> ActionLogic { get; }
        public KeyFrame EndKeyFrame { get; }
        public bool IsEnabled { get; set; } = true;
        public KeyFrame KeyFrame { get; }
        public int Tag { get; }

        #endregion Public Properties
    }
}