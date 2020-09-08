using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.Projectiles.SwordandShield
{
    public sealed class SNSSwipe1
        : ModProjectile
    {
        //private int[] FrameTimings = new int[7] { 32, 22, 18, 14, 12, 10, 0 };
        private int[] FrameTimings = new int[7] { 26, 16, 14, 12, 11, 10, 0 };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SNSSwipe1");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {   
            projectile.width = 110;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.ownerHitCheck = true;
            projectile.aiStyle = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 14;
        }

        public override void AI()
        {
            Player Owner = Main.player[projectile.owner];

            projectile.position.X = Owner.MountedCenter.X - (projectile.width / 2) - (6 * Owner.direction);
            projectile.position.Y = Owner.MountedCenter.Y - (projectile.height / 2) - 10;

            projectile.direction = Owner.direction;
            projectile.spriteDirection = Owner.direction;
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation;

            if (Owner.itemAnimation < FrameTimings[1] && Owner.itemAnimation >= FrameTimings[2])
            {
                projectile.frame = 0;
            }
            else if (Owner.itemAnimation <= FrameTimings[2] && Owner.itemAnimation >= FrameTimings[3])
            {
                projectile.frame = 1;
            }
            else if (Owner.itemAnimation <= FrameTimings[3] && Owner.itemAnimation >= FrameTimings[4])
            {
                projectile.frame = 2;
            }
            else if (Owner.itemAnimation <= FrameTimings[4] && Owner.itemAnimation >= FrameTimings[5])
            {
                projectile.frame = 3;
            }
            else if (Owner.itemAnimation <= FrameTimings[5] && Owner.itemAnimation > FrameTimings[6])
            {
                projectile.frame = 4;
            }
            else if (Owner.itemAnimation == FrameTimings[6])
            {
                projectile.Kill();
            }
        }
        
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
    }
}
