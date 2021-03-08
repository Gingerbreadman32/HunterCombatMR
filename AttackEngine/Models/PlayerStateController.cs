using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Services;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerStateController
    {
        #region Private Fields

        private Animator _actionAnimator;

        #endregion Private Fields

        #region Public Constructors

        // @@info Will have to test to see if this works or not (might break multi)
        public PlayerStateController(HunterCombatPlayer player)
        {
            _actionAnimator = new Animator();
            State = PlayerState.Neutral;
            ActionState = AttackState.NotAttacking;
            Player = player;
            ActionHistory = new SortedList<int, string>();
            ComboManager = new ComboSequenceManager();
        }

        #endregion Public Constructors

        #region Public Properties
        
        public SortedList<int, string> ActionHistory { get; set; }

        public HunterCombatPlayer Player { get; }

        public ComboSequenceManager ComboManager { get; }

        public Attack CurrentAction { get; private set; }

        public AttackState ActionState { get; set; }
        public PlayerState State { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void KillAttackSequence()
        {
            _actionAnimator.ResetAnimation(false);
            CurrentAction = null;
        }

        public void SetNewAction(Attack action)
        {
            CurrentAction = action;
            AnimatorSetup();
        }

        public void Update()
        {


            if (CurrentAction != null)
            {
                var currentKeyFrame = _actionAnimator.GetCurrentKeyFrame();
                var currentFrame = _actionAnimator.CurrentFrame;
                foreach(var keyFrameEvent in CurrentAction.KeyFrameEvents.Where(x => x.StartKeyFrame <= currentKeyFrame 
                        && x.EndKeyFrame >= currentKeyFrame
                        && x.IsEnabled).OrderBy(x => x.Tag))
                {
                    CurrentAction.ActionParameters = keyFrameEvent.ActionLogic.ActionLogic(Player, 
                        currentFrame, 
                        (currentFrame - currentKeyFrame.StartingFrameIndex),
                        CurrentAction.ActionParameters);
                }

                AdvanceAnimator();
            }

            // @@warn Not working, might be because of constructor, look into this
            if (Player.player != null)
                State = PlayerSetStateLogic(Player.player);
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
                CurrentAction.FrameProfile);
        }

        private PlayerState PlayerSetStateLogic(Player player)
        {
            // @@info Switch this up with a dictionary run through so it looks cleaner.
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