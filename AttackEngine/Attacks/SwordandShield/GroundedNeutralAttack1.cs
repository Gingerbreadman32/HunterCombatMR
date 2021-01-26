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
    public sealed class GroundedNeutralAttack1
        : Attack
    {
        public GroundedNeutralAttack1(string name) 
            : base(name)
        {
        }

        public override IEnumerable<AttackProjectile> AttackProjectiles
            => new List<AttackProjectile>() { ModContent.GetInstance<SNSSwipe1>() };

        protected override KeyFrameProfile FrameProfile
            => new KeyFrameProfile(6, 4, new Dictionary<int, int>() { { 0, 10 }, { 3, 2 }, { 4, 2 }, { 5, 10 } });

        protected override void UpdateLogic()
        {
            if (Animation.CheckCurrentKeyFrame(0))
            {
                if (Animation.CurrentFrame.Equals(1))
                {
                    ActionObject.State = Enumerations.PlayerState.AttackStartup;
                }

                ItemAssociated.noUseGraphic = false;
                ActionObject.player.itemRotation = MathHelper.ToRadians(-90 * ActionObject.player.direction);
                ActionObject.player.bodyFrame.Y = ActionObject.player.bodyFrame.Height;

                var CurrentHandOffset = Main.OffsetsPlayerOnhand[ActionObject.player.bodyFrame.Y / 56];
                if (ActionObject.player.direction != 1)
                {
                    CurrentHandOffset.X = ActionObject.player.bodyFrame.Width - CurrentHandOffset.X;
                }
                if (ActionObject.player.gravDir != 1f)
                {
                    CurrentHandOffset.Y = ActionObject.player.bodyFrame.Height - CurrentHandOffset.Y;
                }
                CurrentHandOffset -= new Vector2(ActionObject.player.bodyFrame.Width - ActionObject.player.width, ActionObject.player.bodyFrame.Height - 42) / 2f;
                CurrentHandOffset += new Vector2(8 * ActionObject.player.direction, 10);

                ActionObject.player.itemLocation = ActionObject.player.position + CurrentHandOffset;
            }
            else if (Animation.CheckCurrentKeyFrame(1) && Animation.CheckCurrentKeyFrameProgress(0))
            {
                ActionObject.State = Enumerations.PlayerState.ActiveAttack;
                ActionObject.player.bodyFrame.Y = ActionObject.player.bodyFrame.Height * 2;

                ActionObject.ActiveProjectiles = new List<string>() { AttackProjectiles.First().projectile.Name };
                Projectile.NewProjectileDirect(new Vector2(ActionObject.player.MountedCenter.X - (AttackProjectiles.First().projectile.width / 2) - (6 * ActionObject.player.direction),
                        ActionObject.player.MountedCenter.Y - (AttackProjectiles.First().projectile.height / 2) - 10),
                    new Vector2(0, 0),
                    AttackProjectiles.First().projectile.type,
                    ItemAssociated.damage,
                    ItemAssociated.knockBack,
                    ActionObject.player.whoAmI);

                ItemAssociated.noUseGraphic = true;
            }
            else if (Animation.CheckCurrentKeyFrame(2))
            {
                ActionObject.player.bodyFrame.Y = ActionObject.player.bodyFrame.Height * 3;
            }
            else if (Animation.CheckCurrentKeyFrame(3))
            {
                ActionObject.State = Enumerations.PlayerState.AttackRecovery;
                ActionObject.player.bodyFrame.Y = ActionObject.player.bodyFrame.Height * 4;
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