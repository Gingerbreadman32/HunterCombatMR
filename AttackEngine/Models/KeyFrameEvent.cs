using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;

namespace HunterCombatMR.AttackEngine.Models
{
    public class KeyFrameEvent<TEntity, TActionType> where TEntity
        : IAnimatedEntity<TActionType> where TActionType
        : Animation<TEntity, TActionType>
    {
        #region Public Constructors

        public KeyFrameEvent(int tag,
            ActionLogicMethod<TEntity, TActionType> actionLogic)
        {
            Tag = tag;
            ActionLogic = actionLogic;
        }

        #endregion Public Constructors

        #region Public Properties

        public ActionLogicMethod<TEntity, TActionType> ActionLogic { get; }
        public bool IsEnabled { get; set; } = true;
        public int Tag { get; }

        #endregion Public Properties
    }
}
