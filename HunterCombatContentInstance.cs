namespace HunterCombatMR
{
    public abstract class HunterCombatContentInstance
    {
        #region Public Properties

        public virtual string InternalName { get; set; }

        #endregion Public Properties

        #region Public Methods

        public abstract T Duplicate<T>(string name) where T : HunterCombatContentInstance;

        #endregion Public Methods
    }
}