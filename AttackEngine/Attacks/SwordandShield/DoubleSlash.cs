using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Attacks.SwordandShield
{
    public sealed class DoubleSlash
        : Attack
    {
        #region Public Constructors

        public DoubleSlash(string name)
            : base(name)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public override IEnumerable<AttackProjectile> AttackProjectiles => new List<AttackProjectile>();

        #endregion Public Properties

        #region Protected Properties

        protected override KeyFrameProfile FrameProfile => new KeyFrameProfile(1, 100);

        #endregion Protected Properties

        #region Protected Methods

        protected override void UpdateLogic()
        {
        }

        #endregion Protected Methods
    }
}