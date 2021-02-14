using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class Attack
        : ActionBase<HunterCombatPlayer, PlayerActionAnimation>
    {
        #region Public Constructors

        public Attack(string name)
        {
            Name = name;
            AttackProjectiles = new List<AttackProjectile>();
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<AttackProjectile> AttackProjectiles { get; set; }
        public bool IsActive { get; set; } = false;

        #endregion Public Properties

    }
}