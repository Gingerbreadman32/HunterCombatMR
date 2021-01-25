using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class Attack
    {
        #region Public Constructors

        public Attack()
        {
            Animation = new AnimatedData();
            Name = "Default";
            SetupAttack();
        }

        public Attack(string name)
        {
            Animation = new AnimatedData();
            Name = name;
            SetupAttack();
        }

        public Attack(string name,
            AnimatedData animation,
            Player player,
            Item item)
        {
            Name = name;
            Animation = animation;
            PerformingPlayer = player;
            ItemAssociated = item;
            SetupAttack();
        }

        #endregion Public Constructors

        #region Public Properties

        public AnimatedData Animation { get; set; }

        public IEnumerable<PlayerActionAnimation> PlayerAnimations { get; set; }

        public abstract IEnumerable<AttackProjectile> AttackProjectiles { get; }

        protected abstract KeyFrameProfile FrameProfile { get; }
        public bool IsActive { get; set; } = false;
        public Item ItemAssociated { get; set; }
        public string Name { get; set; }
        public Player PerformingPlayer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public virtual void Advance()
        {
            if (Animation.IsPlaying && Animation.CurrentFrame.Equals(Animation.GetFinalFrame()))
            {
                KillAttack();
            }
            else
            {
                Animation.AdvanceFrame();
            }
        }

        public virtual void KillAttack()
        {
            PerformingPlayer.itemAnimation = 0;
            PerformingPlayer.itemTime = 0;
            PerformingPlayer.GetModPlayer<HunterCombatPlayer>().State = Enumerations.PlayerState.Standing;
            Animation.ResetAnimation(true);
            IsActive = false;
        }

        public virtual void SetOwners(Player player,
            Item item)
        {
            PerformingPlayer = player;
            ItemAssociated = item;
        }

        public virtual void SetupAttack()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(Animation, FrameProfile);
        }

        public virtual void Update()
        {
            UpdateLogic();

            Advance();
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract void UpdateLogic();

        #endregion Protected Methods
    }
}