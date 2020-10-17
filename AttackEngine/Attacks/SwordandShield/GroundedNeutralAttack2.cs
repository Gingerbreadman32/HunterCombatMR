using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Projectiles.SwordandShield;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.AttackEngine.Attacks.SwordandShield
{
    public sealed class GroundedNeutralAttack2
        : Attack
    {
        public GroundedNeutralAttack2()
            : base()
        {
            Name = "SNS-LMB2";
        }

        public override void SetupAttackProjectiles()
        {
            AttackProjectiles = new List<AttackProjectile>() { ModContent.GetModProjectile(ModContent.ProjectileType<SNSSwipe2>()) as AttackProjectile };
        }

        public override void SetupKeyFrameProfile()
        {
            FrameProfile = new KeyFrameProfile(1, 19);
        }

        protected override void UpdateLogic()
        {
            if (Animation.CurrentFrame.Equals(0))
            {
                if (!Animation.IsPlaying)
                    Animation.StartAnimation();

                PerformingPlayer.GetModPlayer<HunterCombatPlayer>().ActiveProjectiles = new List<string>() { AttackProjectiles.First().projectile.Name };
                Projectile.NewProjectileDirect(new Vector2(PerformingPlayer.MountedCenter.X - (AttackProjectiles.First().projectile.width / 2) - (6 * PerformingPlayer.direction),
                        PerformingPlayer.MountedCenter.Y - (AttackProjectiles.First().projectile.height / 2) - 10),
                    new Vector2(0, 0),
                    AttackProjectiles.First().projectile.type,
                    ItemAssociated.damage,
                    ItemAssociated.knockBack,
                    PerformingPlayer.whoAmI);
            }
        }
    }
}