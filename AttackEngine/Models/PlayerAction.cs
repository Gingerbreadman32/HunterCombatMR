using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class PlayerAction
        : CustomAction<HunterCombatPlayer, Player>
    {
        #region Public Constructors

        public PlayerAction(string name,
            string displayName = "")
            : base(name, displayName)
        {
            Animations = new SortedList<int, IAnimation>();
            Projectiles = new List<AttackProjectile>();
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<AttackProjectile> Projectiles { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override T Duplicate<T>(string name)
        {
            T clone = base.Duplicate<T>(name);
            var playerAction = clone as PlayerAction;
            playerAction.Projectiles = new List<AttackProjectile>(Projectiles);
            playerAction.Animations = new SortedList<int, IAnimation>(Animations);
            playerAction.KeyFrameProfile = new KeyFrameProfile(KeyFrameProfile);
            playerAction.KeyFrameEvents = new List<KeyFrameEvent<HunterCombatPlayer, Player>>(KeyFrameEvents);
            playerAction.DefaultParameters = new Dictionary<string, string>(DefaultParameters);

            return clone;
        }

        #endregion Public Methods
    }
}
