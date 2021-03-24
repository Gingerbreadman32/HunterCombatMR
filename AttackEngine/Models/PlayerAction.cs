using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class PlayerAction
        : CustomAction<HunterCombatPlayer, PlayerActionAnimation>
    {
        #region Public Constructors

        public PlayerAction(string name,
            string displayName = "")
            : base(name, displayName)
        {
            Animations = new List<PlayerActionAnimation>();
            AttackProjectiles = new List<AttackProjectile>();
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<AttackProjectile> AttackProjectiles { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override T Duplicate<T>(string name)
        {
            T clone = base.Duplicate<T>(name);
            var playerAction = clone as PlayerAction;
            playerAction.AttackProjectiles = new List<AttackProjectile>(AttackProjectiles);
            playerAction.Animations = new List<PlayerActionAnimation>(Animations);
            playerAction.KeyFrameProfile = new KeyFrameProfile(KeyFrameProfile);
            playerAction.KeyFrameEvents = new List<KeyFrameEvent<HunterCombatPlayer, PlayerActionAnimation>>(KeyFrameEvents);
            playerAction.DefaultParameters = new Dictionary<string, string>(DefaultParameters);

            return clone;
        }

        #endregion Public Methods
    }
}