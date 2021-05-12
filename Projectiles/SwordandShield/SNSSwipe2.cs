using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.Projectiles.SwordandShield
{
    public sealed class SNSSwipe2
        : AttackProjectile
    {
        private const int _sprites = 7;

        public SNSSwipe2(ProjectileAnimation animation) 
            : base(animation)
        {
        }

        /*
        public override void SetupKeyFrameProfile()
        {
            KeyFrameProfile = new KeyFrameProfile(_sprites, 2, new SortedList<int, FrameLength>() 
            { 
                { 3, 1.ToFLength() }, 
                { 4, 1.ToFLength() }, 
                { 5, 1.ToFLength() }, 
                { 6, 10.ToFLength() } 
            });
        }*/

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SNSSwipe2");
            Main.projFrames[projectile.type] = _sprites;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 110;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.ownerHitCheck = true;
            projectile.aiStyle = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = CurrentAnimation.AnimationData.TotalFrames;
        }

        public override void AI()
        {
            Player Owner = Main.player[projectile.owner];

            if (!Owner.GetModPlayer<HunterCombatPlayer>().ActiveProjectiles.Contains(projectile.Name))
                projectile.Kill();

            projectile.position = new Vector2(Owner.MountedCenter.X - (projectile.width / 2) - (6 * Owner.direction),
                Owner.MountedCenter.Y - (projectile.height / 2) - 10);

            projectile.direction = Owner.direction;
            projectile.spriteDirection = Owner.direction;
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation;
        }
    }
}