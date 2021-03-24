using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.Projectiles.SwordandShield
{
    public sealed class SNSSwipe1
        : AttackProjectile
    {
        private const int _sprites = 5;

        public SNSSwipe1(ProjectileAnimation animation) 
            : base(animation)
        {
        }

        /*public override void SetupKeyFrameProfile()
        {
            KeyFrameProfile = new KeyFrameProfile(_sprites, 4, new SortedList<int, FrameLength>() { { 0, 2.ToFLength() }, { 1, 2.ToFLength() }, { 4, 10.ToFLength() } });
        }*/

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SNSSwipe1");
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
            projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            Player Owner = Main.player[projectile.owner];

            projectile.position = new Vector2(Owner.MountedCenter.X - (projectile.width / 2) - (6 * Owner.direction),
                Owner.MountedCenter.Y - (projectile.height / 2) - 10);

            projectile.direction = Owner.direction;
            projectile.spriteDirection = Owner.direction;
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation;
        }
    }
}