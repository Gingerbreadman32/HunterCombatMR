using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Attacks.SwordandShield
{
    public sealed class RunningSlash
        : Attack
    {
        public override IEnumerable<AttackProjectile> AttackProjectiles => throw new NotImplementedException();

        protected override KeyFrameProfile FrameProfile => throw new NotImplementedException();

        protected override void UpdateLogic()
        {
        }
    }
}