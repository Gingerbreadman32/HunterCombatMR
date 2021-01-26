using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class Attack
        : ActionBase<HunterCombatPlayer, PlayerActionAnimation>
    {
        #region Public Constructors

        public Attack(string name)
        {
            Animation = new AnimatedData();
            Name = name;
            SetupAttack();
        }

        #endregion Public Constructors

        #region Public Properties

        public AnimatedData Animation { get; set; }

        public abstract IEnumerable<AttackProjectile> AttackProjectiles { get; }
        public bool IsActive { get; set; } = false;
        public Item ItemAssociated { get; set; }
        public string Name { get; set; }
        protected abstract KeyFrameProfile FrameProfile { get; }

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
            ActionObject.player.itemAnimation = 0;
            ActionObject.player.itemTime = 0;
            ActionObject.State = Enumerations.PlayerState.Neutral;
            Animation.ResetAnimation(false);
            IsActive = false;
        }

        public virtual void SetOwners(HunterCombatPlayer player,
            Item item)
        {
            ActionObject = player;
            ItemAssociated = item;
        }

        public virtual void SetupAttack()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(Animation, FrameProfile);
        }

        public virtual void Start()
        {
            Animation.StartAnimation();
        }

        public virtual void Update()
        {
            UpdateLogic();

            Advance();
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void SetAnimation(PlayerActionAnimation animation)
        {
            if (Animations.Contains(animation))
                ActionObject.SetCurrentAnimation(animation);
        }

        protected abstract void UpdateLogic();

        #endregion Protected Methods
    }
}