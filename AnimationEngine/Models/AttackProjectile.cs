using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class AttackProjectile
        : ModProjectile,
        IAnimated
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

        public override void SetDefaults()
        {
            projectile.timeLeft = Animation.TotalFrames;
        }

        public override bool PreAI()
        {
            if (!Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles.Contains(projectile.Name))
                projectile.Kill();

            return base.PreAI();
        }

        public abstract void SetupKeyFrameProfile();

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var projectiles = Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles;
            var active = projectiles.Contains(projectile.Name);

            if (!Animation.InProgress && active)
                Play();

            projectile.frame = Animation.KeyFrames.IndexOf(Animation.GetCurrentKeyFrame());

            if (active)
                return base.PreDraw(spriteBatch, lightColor);
            else
                return active;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            base.PostDraw(spriteBatch, lightColor);
            Update();
        }

        public override bool PreKill(int timeLeft)
        {
            Stop();
            var projectiles = Main.player[projectile.owner].GetModPlayer<HunterCombatPlayer>().ActiveProjectiles;

            if (projectiles.Contains(projectile.Name))
                projectiles.Remove(projectile.Name);

            return true;
        }

        public virtual void Initialize()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(Animation, FrameProfile, false);
        }

        public override ModProjectile NewInstance(Projectile projectileClone)
        {
            AttackProjectile obj = (AttackProjectile)base.NewInstance(projectileClone);
            obj.Animation = Animation;
            obj.Hitboxes = Hitboxes;
            obj.projectile.timeLeft = Animation.TotalFrames;
            return obj;
        }

        public void Pause()
        {
            if (Animation.IsPlaying)
                Animation.PauseAnimation();
        }

        public void Play()
        {
            if (!Animation.IsPlaying)
                Animation.StartAnimation();
        }

        public void Stop()
        {
            Animation.StopAnimation();
        }

        public void Restart()
        {
            Animation.ResetAnimation(true);
        }

        public virtual void Update()
        {
            if (Animation.IsPlaying && Animation.CurrentFrame < Animation.GetFinalFrame())
            {
                Animation.AdvanceFrame();
            }
        }

        public void UpdateKeyFrameLength(int keyFrameIndex, int frameAmount, bool setAmount = false, bool setDefault = false)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLoopType(LoopStyle newLoopType)
        {
            throw new System.NotImplementedException();
        }
    }
}