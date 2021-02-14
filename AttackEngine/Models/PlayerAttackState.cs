using HunterCombatMR.AnimationEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerAttackState
    {
        #region Private Fields

        private Animator _currentAttackAnimator;

        #endregion Private Fields

        #region Public Properties

        public Attack CurrentAttackReference { get; set; }

        #endregion Public Properties


        #region Public Methods

        public void Advance()
        {
            if (_currentAttackAnimator.CurrentFrame.Equals(_currentAttackAnimator.GetFinalFrame()))
            {
                KillAttackSequence();
            }
            else
            {
                _currentAttackAnimator.AdvanceFrame();
            }
        }

        public void KillAttackSequence()
        {
            _currentAttackAnimator.ResetAnimation(false);
            CurrentAttackReference = null;
        }

        public void SetupAttack()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(_currentAttackAnimator,
                CurrentAttackReference.FrameProfile);
        }

        public void Update(HunterCombatPlayer player)
        {
            CurrentAttackReference.LogicMethods[_currentAttackAnimator.GetCurrentKeyFrameIndex()].ActionLogic(player, 
                _currentAttackAnimator.CurrentFrame,
                _currentAttackAnimator.GetCurrentKeyFrame(),
                CurrentAttackReference.GetActionParameters(player));

            Advance();
        }

        #endregion Public Methods
    }
}
