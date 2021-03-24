using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class AttackProjectile
        : ModProjectile,
        IAnimatedEntity<ProjectileAnimation>
    {
        #region Private Fields

        private ProjectileAnimation _animation;

        #endregion Private Fields

        #region Public Constructors

        public AttackProjectile(ProjectileAnimation animation)
            : base()
        {
            Hitboxes = new List<Hitbox>();
            _animation = animation;
        }

        #endregion Public Constructors

        #region Public Properties

        public ProjectileAnimation CurrentAnimation { get => _animation; }
        public ICollection<Hitbox> Hitboxes { get; set; }

        #endregion Public Properties

        #region Public Methods

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
            base.PostDraw(spriteBatch, lightColor);
            Update();
        }

        public override bool PreAI()
        {
            if (!Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles.Contains(projectile.Name))
                projectile.Kill();

            return base.PreAI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var projectiles = Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles;
            var active = projectiles.Contains(projectile.Name);

            projectile.frame = _animation.AnimationData.CurrentKeyFrameIndex;

            if (active)
                return base.PreDraw(spriteBatch, lightColor);
            else
                return active;
        }

        public override bool PreKill(int timeLeft)
        {
            _animation.Stop();
            var projectiles = Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles;

            if (projectiles.Contains(projectile.Name))
                projectiles.Remove(projectile.Name);

            return true;
        }

        public bool SetCurrentAnimation(ProjectileAnimation newAnimation, bool newFile = false)
        {
            if (newAnimation == null)
            {
                _animation = null;
                return true;
            }

            if (newAnimation == CurrentAnimation)
                return true;

            _animation = newAnimation;

            return CurrentAnimation != null;
        }

        public override void SetDefaults()
        {
            projectile.timeLeft = CurrentAnimation.AnimationData.TotalFrames;
        }

        public virtual void Update()
        {
            _animation.Update(_animation.AnimationData);
        }

        #endregion Public Methods
    }
}