using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerStateController
    {
        #region Private Fields

        private Animator _actionAnimator;

        #endregion Private Fields

        #region Public Constructors

        public PlayerStateController()
        {
            _actionAnimator = new Animator();
            ActionReference = null;
            State = PlayerState.Neutral;
            ActionState = AttackState.NotAttacking;
        }

        #endregion Public Constructors

        #region Public Properties

        public Attack ActionReference { get; private set; }

        public AttackState ActionState { get; set; }
        public PlayerState State { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void KillAttackSequence()
        {
            _actionAnimator.ResetAnimation(false);
            ActionReference = null;
        }

        public void SetNewAction(Attack action)
        {
            ActionReference = action;
            AnimatorSetup();
        }

        public void Update(HunterCombatPlayer player)
        {
            if (ActionReference != null)
            {
                ActionReference.LogicMethods[_actionAnimator.GetCurrentKeyFrameIndex()].ActionLogic(player,
                    _actionAnimator.CurrentFrame,
                    _actionAnimator.GetCurrentKeyFrame(),
                    ActionReference.GetActionParameters(player));

                AdvanceAnimator();
            }

            State = PlayerSetStateLogic(player.player);
        }

        #endregion Public Methods

        #region Private Methods

        private void AdvanceAnimator()
        {
            _actionAnimator.AdvanceFrame();

            if (!_actionAnimator.InProgress)
                KillAttackSequence();
        }

        private void AnimatorSetup()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(_actionAnimator,
                ActionReference.FrameProfile);
        }

        private PlayerState PlayerSetStateLogic(Player player)
        {
            // Switch this up with a dictionary run through.
            if (State == PlayerState.Dead)
                return PlayerState.Dead;

            if (player.IsPlayerJumping())
                return PlayerState.Jumping;

            if (player.IsPlayerAerial())
                return PlayerState.Aerial;

            if (player.IsPlayerWalking())
                return PlayerState.Walking;

            return PlayerState.Neutral;
        }

        #endregion Private Methods
    }
}