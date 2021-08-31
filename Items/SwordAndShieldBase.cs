using HunterCombatMR.Constants;

namespace HunterCombatMR.Items
{
    public abstract class SwordAndShieldBase
        : WeaponBase
    {
        #region Public Properties

        public override string MoveSet { get; protected set; } = MoveSetNames.SwordAndShield;

        #endregion Public Properties
    }
}