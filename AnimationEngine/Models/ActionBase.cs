using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class ActionBase<TObject, TAnimationType> where TObject
        : IAnimated<TAnimationType> where TAnimationType
        : Animation
    {
        #region Public Properties

        public IEnumerable<TAnimationType> Animations { get; protected set; }

        public abstract KeyFrameProfile FrameProfile { get; }
        public abstract SortedList<int, ActionLogicMethod<TObject, TAnimationType>> LogicMethods { get; }
        public string Name { get; set; }

        #endregion Public Properties

        #region Public Methods

        public virtual IDictionary<string, string> GetActionParameters(TObject @object)
            => new Dictionary<string, string>();

        #endregion Public Methods
    }
}