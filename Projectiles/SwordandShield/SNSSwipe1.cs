﻿using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.Projectiles.SwordandShield
{
    public sealed class SNSSwipe1
        : AttackProjectile
    {
        private const int _sprites = 5;

        public override void SetupKeyFrameProfile()
        {
            FrameProfile = new KeyFrameProfile(_sprites, 4, new Dictionary<int, int>() { { 0, 2 }, { 1, 2 }, { 4, 10 } });
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SNSSwipe1");
            Main.projFrames[projectile.type] = _sprites;
        }

        public override void SetDefaults()
        {
            projectile.width = 110;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 100;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.ownerHitCheck = true;
            projectile.aiStyle = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            PreAnimate();

            Player Owner = Main.player[projectile.owner];

            if (!Owner.GetModPlayer<HunterCombatPlayer>().ActiveProjectiles.Contains(projectile.Name))
                projectile.Kill();

            projectile.position = new Vector2(Owner.MountedCenter.X - (projectile.width / 2) - (6 * Owner.direction),
                Owner.MountedCenter.Y - (projectile.height / 2) - 10);

            projectile.direction = Owner.direction;
            projectile.spriteDirection = Owner.direction;
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation;

            PostAnimate();
        }
    }
}