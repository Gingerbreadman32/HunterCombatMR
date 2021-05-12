using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.Models
{
    public abstract class AttackProjectile
        : ModProjectile,
        IAnimatedEntity<ProjectileAnimation>
    {
        private ProjectileAnimation _animation;

        public AttackProjectile(ProjectileAnimation animation)
            : base()
        {
            Hitboxes = new List<Hitbox>();
            _animation = animation;
        }

        public ProjectileAnimation CurrentAnimation { get => _animation; }
        public ICollection<Hitbox> Hitboxes { get; set; }

        public bool IsPlayerActive { get => Main.player[projectile.owner]?.GetModPlayer<HunterCombatPlayer>().ActiveProjectiles?.Contains(projectile.Name) ?? false; }

        public override ModProjectile NewInstance(Projectile projectileClone)
        {
            AttackProjectile obj = (AttackProjectile)base.NewInstance(projectileClone);
            obj._animation = _animation;
            obj.Hitboxes = Hitboxes;
            obj.SetDefaults();
            return obj;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (IsPlayerActive)
                _animation.Update();
        }

        public override bool PreAI()
        {
            if (!Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles.Contains(projectile.Name))
                projectile.Kill();

            return base.PreAI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.frame = _animation.AnimationData.CurrentKeyFrameIndex;

            return IsPlayerActive;
        }

        public override bool PreKill(int timeLeft)
        {
            _animation.AnimationData.Stop(false);
            var projectiles = Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles;

            if (projectiles.Contains(projectile.Name))
                projectiles.Remove(projectile.Name);

            return true;
        }

        public bool SetCurrentAnimation(ICustomAnimation newAnimation, bool newFile = false)
        {
            if (newAnimation == null)
            {
                _animation = null;
                return true;
            }

            if (newAnimation == CurrentAnimation)
                return true;

            _animation = (ProjectileAnimation)newAnimation;

            return CurrentAnimation != null;
        }

        public override void SetDefaults()
        {
            projectile.timeLeft = CurrentAnimation.AnimationData.TotalFrames;
            _animation.AnimationData.Play();
        }
    }
}