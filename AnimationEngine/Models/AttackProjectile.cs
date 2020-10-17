using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Services;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class AttackProjectile
        : ModProjectile
    {
        public IAnimation Animation { get; set; }

        public ICollection<Hitbox> Hitboxes { get; set; }

        public KeyFrameProfile FrameProfile { get; set; }

        public AttackProjectile()
            : base()
        {
            Animation = new StandardAnimation();
            Hitboxes = new List<Hitbox>();
            SetupKeyFrameProfile();
        }

        public abstract void SetupKeyFrameProfile();

        /// <summary>
        /// Call at the beginning of the AI method for the projectile
        /// </summary>
        public virtual void PreAnimate()
        {
            if (!Animation.IsInitialized)
            {
                HunterCombatMR.AnimationKeyFrameManager.FillAnimationKeyFrames(Animation, FrameProfile);
                projectile.timeLeft = Animation.TotalFrames;
                projectile.localNPCHitCooldown = Animation.TotalFrames;
            }
            projectile.frame = Animation.GetCurrentKeyFrame().SpriteIndex;
        }

        /// <summary>
        /// Call at the end of the AI method for the projectile
        /// </summary>
        public virtual void PostAnimate()
        {
            if (Animation.IsPlaying && Animation.CurrentFrame < (Animation.TotalFrames - 1))
                Animation.AdvanceFrame();
            else if (Animation.CurrentFrame == (Animation.TotalFrames - 1))
                projectile.Kill();
        }

        public override bool PreKill(int timeLeft)
        {
            Player Owner = Main.player[projectile.owner];

            if (Owner.GetModPlayer<HunterCombatPlayer>().ActiveProjectiles.Contains(projectile.Name))
                Owner.GetModPlayer<HunterCombatPlayer>().ActiveProjectiles.Remove(projectile.Name);

            return true;
        }
    }
}