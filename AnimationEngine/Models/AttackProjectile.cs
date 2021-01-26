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
        IAnimation
    {
        public AnimatedData AnimationData { get; set; }

        public ICollection<Hitbox> Hitboxes { get; set; }

        public KeyFrameProfile FrameProfile { get; set; }

        public AttackProjectile()
            : base()
        {
            AnimationData = new AnimatedData();
            Hitboxes = new List<Hitbox>();
            SetupKeyFrameProfile();
        }

        public override void SetDefaults()
        {
            projectile.timeLeft = AnimationData.TotalFrames;
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

            if (!AnimationData.InProgress && active)
                Play();

            projectile.frame = AnimationData.KeyFrames.IndexOf(AnimationData.GetCurrentKeyFrame());

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
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(AnimationData, FrameProfile, false);
        }

        public override ModProjectile NewInstance(Projectile projectileClone)
        {
            AttackProjectile obj = (AttackProjectile)base.NewInstance(projectileClone);
            obj.AnimationData = AnimationData;
            obj.Hitboxes = Hitboxes;
            obj.projectile.timeLeft = AnimationData.TotalFrames;
            return obj;
        }

        public void Pause()
        {
            if (AnimationData.IsPlaying)
                AnimationData.PauseAnimation();
        }

        public void Play()
        {
            if (!AnimationData.IsPlaying)
                AnimationData.StartAnimation();
        }

        public void Stop()
        {
            AnimationData.StopAnimation();
        }

        public void Restart()
        {
            AnimationData.ResetAnimation(true);
        }

        public virtual void Update()
        {
            if (AnimationData.IsPlaying && AnimationData.CurrentFrame < AnimationData.GetFinalFrame())
            {
                AnimationData.AdvanceFrame();
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