using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Attacks.SwordandShield
{
    public sealed class RunningSlash
        : Attack
    {
        #region Public Constructors

        public RunningSlash(string name)
            : base(name)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public override SortedList<int, ActionLogicMethod<HunterCombatPlayer, PlayerActionAnimation>> LogicMethods => throw new System.NotImplementedException();

        public override KeyFrameProfile FrameProfile => throw new System.NotImplementedException();

        #endregion Public Properties

    }
}