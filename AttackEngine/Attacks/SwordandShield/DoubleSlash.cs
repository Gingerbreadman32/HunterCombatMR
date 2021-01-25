﻿using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Attacks.SwordandShield
{
    public sealed class DoubleSlash
        : Attack
    {
        public override IEnumerable<AttackProjectile> AttackProjectiles { get; }
        protected override KeyFrameProfile FrameProfile { get; }

        protected override void UpdateLogic()
        {
        }
    }
}