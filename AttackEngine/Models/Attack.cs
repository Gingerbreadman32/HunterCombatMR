using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class Attack
        : CustomAction<HunterCombatPlayer, PlayerActionAnimation>
    {
        #region Public Constructors

        public Attack(string name)
        {
            Name = name;
            AttackProjectiles = new List<AttackProjectile>();
            Animations = new List<PlayerActionAnimation>();
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<AttackProjectile> AttackProjectiles { get; set; }

        #endregion Public Properties

    }
}