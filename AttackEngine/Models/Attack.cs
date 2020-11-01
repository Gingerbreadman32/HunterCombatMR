using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class Attack
        : IComboable
    {
        public IAnimation Animation { get; set; }

        public IEnumerable<AttackProjectile> AttackProjectiles { get; set; }

        public Player PerformingPlayer { get; set; }

        public Item ItemAssociated { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; } = false;

        public IEnumerable<ComboRoute> Routes { get; set; }

        public KeyFrameProfile FrameProfile { get; set; }

        public Attack()
        {
            Animation = new StandardAnimation();
            AttackProjectiles = new List<AttackProjectile>();
            Name = "Default";
            EstablishRoutes();
            SetupKeyFrameProfile();
            SetupAttackProjectiles();
        }

        public Attack(Player player,
            Item item)
        {
            Animation = new StandardAnimation();
            AttackProjectiles = new List<AttackProjectile>();
            Name = "Default";
            PerformingPlayer = player;
            ItemAssociated = item;
            EstablishRoutes();
            SetupKeyFrameProfile();
            SetupAttackProjectiles();
        }

        public Attack(string name,
            IAnimation animation,
            ICollection<AttackProjectile> attackProjectiles,
            Player player,
            Item item)
        {
            Name = name;
            Animation = animation;
            AttackProjectiles = new List<AttackProjectile>(attackProjectiles);
            PerformingPlayer = player;
            ItemAssociated = item;
            EstablishRoutes();
            SetupKeyFrameProfile();
            SetupAttackProjectiles();
        }

        public abstract void SetupAttackProjectiles();

        public abstract void SetupKeyFrameProfile();

        protected abstract void UpdateLogic();

        /// <inheritdoc/>
        public virtual void EstablishRoutes()
        {
        }

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

        public virtual void Update()
        {
            UpdateLogic();

            Advance();
        }

        public virtual void SetOwners(Player player,
            Item item)
        {
            PerformingPlayer = player;
            ItemAssociated = item;
        }

        public void CopyRoutes(IEnumerable<ComboRoute> newRoutes)
        {
            Routes = new List<ComboRoute>(newRoutes);
        }
    }
}