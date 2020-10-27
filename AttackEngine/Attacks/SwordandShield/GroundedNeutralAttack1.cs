using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Projectiles.SwordandShield;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.AttackEngine.Attacks.SwordandShield
{
    public sealed class GroundedNeutralAttack1
        : Attack
    {
        public GroundedNeutralAttack1()
            : base()
        {
            Name = "SNS-LMB1";
        }

        public override void SetupAttackProjectiles()
        {
            AttackProjectiles = new List<AttackProjectile>() { ModContent.GetInstance<SNSSwipe1>() };
        }

        public override void SetupKeyFrameProfile()
        {
            FrameProfile = new KeyFrameProfile(6, 4, new Dictionary<int, int>() { { 0, 10 }, { 3, 2 }, { 4, 2 }, { 5, 10 } });
        }

        /// <inheritdoc/>
        public override void EstablishRoutes()
        {
            Routes = new List<ComboRoute>()
            {
                new ComboRoute("SNS-LMB2", 5, 10, Enumerations.ComboInputs.StandardAttack, 1)
            };
        }

        protected override void UpdateLogic()
        {
            if (Animation.CheckCurrentKeyFrame(0))
            {
                if (Animation.CurrentFrame.Equals(1))
                {
                    PerformingPlayer.GetModPlayer<HunterCombatPlayer>().State = Enumerations.PlayerState.AttackStartup;
                }

                ItemAssociated.noUseGraphic = false;
                PerformingPlayer.itemRotation = MathHelper.ToRadians(-90 * PerformingPlayer.direction);
                PerformingPlayer.bodyFrame.Y = PerformingPlayer.bodyFrame.Height;

                var CurrentHandOffset = Main.OffsetsPlayerOnhand[PerformingPlayer.bodyFrame.Y / 56];
                if (PerformingPlayer.direction != 1)
                {
                    CurrentHandOffset.X = PerformingPlayer.bodyFrame.Width - CurrentHandOffset.X;
                }
                if (PerformingPlayer.gravDir != 1f)
                {
                    CurrentHandOffset.Y = PerformingPlayer.bodyFrame.Height - CurrentHandOffset.Y;
                }
                CurrentHandOffset -= new Vector2(PerformingPlayer.bodyFrame.Width - PerformingPlayer.width, PerformingPlayer.bodyFrame.Height - 42) / 2f;
                CurrentHandOffset += new Vector2(8 * PerformingPlayer.direction, 10);

                PerformingPlayer.itemLocation = PerformingPlayer.position + CurrentHandOffset;
            }
            else if (Animation.CheckCurrentKeyFrame(1) && Animation.CheckCurrentKeyFrameProgress(0))
            {
                PerformingPlayer.GetModPlayer<HunterCombatPlayer>().State = Enumerations.PlayerState.ActiveAttack;
                PerformingPlayer.bodyFrame.Y = PerformingPlayer.bodyFrame.Height * 2;

                PerformingPlayer.GetModPlayer<HunterCombatPlayer>().ActiveProjectiles = new List<string>() { AttackProjectiles.First().projectile.Name };
                Projectile.NewProjectileDirect(new Vector2(PerformingPlayer.MountedCenter.X - (AttackProjectiles.First().projectile.width / 2) - (6 * PerformingPlayer.direction),
                        PerformingPlayer.MountedCenter.Y - (AttackProjectiles.First().projectile.height / 2) - 10),
                    new Vector2(0, 0),
                    AttackProjectiles.First().projectile.type,
                    ItemAssociated.damage,
                    ItemAssociated.knockBack,
                    PerformingPlayer.whoAmI);

                ItemAssociated.noUseGraphic = true;
            }
            else if (Animation.CheckCurrentKeyFrame(2))
            {
                PerformingPlayer.bodyFrame.Y = PerformingPlayer.bodyFrame.Height * 3;
            }
            else if (Animation.CheckCurrentKeyFrame(3))
            {
                PerformingPlayer.GetModPlayer<HunterCombatPlayer>().State = Enumerations.PlayerState.AttackRecovery;
                PerformingPlayer.bodyFrame.Y = PerformingPlayer.bodyFrame.Height * 4;
            }
            /*
            else if (Animation.CheckCurrentKeyFrame(5))
            {
                if (Animation.CheckCurrentKeyFrameProgress(0) && comboable == false && CurrentAttack == LMB1)
                {
                    comboable = true;
                }
                if (comboable && (PlayerInput.Triggers.JustPressed.MouseLeft || _comboDebug))
                {
                    CurrentAttack = LMB2;
                    comboable = false;
                    Animation.ResetAnimation(false);
                }
            }
            */
        }
    }
}