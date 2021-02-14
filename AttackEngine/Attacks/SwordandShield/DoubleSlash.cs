using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Attacks.SwordandShield
{
    public sealed class DoubleSlash
        : Attack
    {
        private const int _keyFrames = 1;

        #region Public Constructors

        public DoubleSlash(string name)
            : base(name)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public override KeyFrameProfile FrameProfile => new KeyFrameProfile(_keyFrames, 100, new int[_keyFrames] { 100 });

        public override SortedList<int, ActionLogicMethod<HunterCombatPlayer, PlayerActionAnimation>> LogicMethods => throw new System.NotImplementedException();

        #endregion Public Properties
    }
}