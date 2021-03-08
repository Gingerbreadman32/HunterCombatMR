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
            InternalName = name;
            Name = name;
            AttackProjectiles = new List<AttackProjectile>();
            Animations = new List<PlayerActionAnimation>();
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<AttackProjectile> AttackProjectiles { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override T Duplicate<T>(string name)
        {
            Attack clone = (Attack)MemberwiseClone();

            clone.InternalName = name;
            clone.AttackProjectiles = new List<AttackProjectile>(AttackProjectiles);
            clone.Animations = new List<PlayerActionAnimation>(Animations);
            clone.FrameProfile = new KeyFrameProfile(FrameProfile);
            clone.KeyFrameEvents = new List<KeyFrameEvent<HunterCombatPlayer, PlayerActionAnimation>>(KeyFrameEvents);
            clone.ActionParameters = new Dictionary<string, string>(ActionParameters);

            return clone as T;
        }

        #endregion Public Methods
    }
}