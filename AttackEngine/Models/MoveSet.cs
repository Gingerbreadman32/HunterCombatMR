using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class MoveSet
        : HunterCombatContentInstance
    {
        #region Public Constructors

        public MoveSet(string name)
        {
            InternalName = name;
            Actions = PopulateActions();
            StartingRoutes = SetStartingRoutes();
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<ComboAction> Actions { get; private set; }

        /// <summary>
        /// The actions that can be used from non-action states.
        /// </summary>
        public IEnumerable<ComboRoute> StartingRoutes { get; private set; }

        #endregion Public Properties

        #region Protected Methods

        protected virtual IEnumerable<ComboAction> PopulateActions()
            => new List<ComboAction>();

        protected virtual IEnumerable<ComboRoute> SetStartingRoutes()
            => new List<ComboRoute>();

        #endregion Protected Methods

        #region Public Methods

        public override T Duplicate<T>(string name)
        {
            MoveSet clone = (MoveSet)MemberwiseClone();

            clone.InternalName = name;
            clone.Actions = clone.PopulateActions();
            clone.StartingRoutes = clone.SetStartingRoutes();

            return clone as T;
        }

        #endregion Public Methods
    }
}