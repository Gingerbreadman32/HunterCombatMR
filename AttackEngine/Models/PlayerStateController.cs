using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerStateController
    {
        #region Private Fields

        private Animator _actionAnimator;
        private MoveSet _currentMoveSet;

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
        }

        #endregion Public Constructors

        #region Public Properties

        public SortedList<int, string> ActionHistory { get; private set; }
        public HunterCombatPlayer Player { get; }
        public ComboAction CurrentAction { get; private set; }
        public AttackState ActionState { get; set; }
        public PlayerState State { get; set; }

        #endregion Public Properties

        #region Public Methods

        public int GetCurrentActionFrame()
            => _actionAnimator.CurrentFrame;

        public void KillAttackSequence()
        {
            _actionAnimator.ResetAnimation(false);
            CurrentAction = null;
        }

        public void SetNewAction(ComboAction action)
        {
            CurrentAction = action;
            SetupAnimator();
        }

        public void Update()
        {
            if (Player.EquippedWeapon != null)
            {
                var nextAction = PlayerActionComboUtils.GetNextAvailableAction(this,
                    GetOrReturnCurrentMoveSet(Player.EquippedWeapon.MoveSet),
                    Player.InputBuffers);

                if (nextAction != CurrentAction)
                    SetNewAction(nextAction);

                if (CurrentAction != null)
                {
                    var currentKeyFrame = _actionAnimator.GetCurrentKeyFrame();
                    var currentFrame = _actionAnimator.CurrentFrame;
                    foreach (var keyFrameEvent in CurrentAction.Attack.KeyFrameEvents.Where(x => (x.KeyFrame.IsKeyFrameActive(currentFrame)
                             || x.EndKeyFrame.IsKeyFrameActive(currentFrame))
                             && x.IsEnabled).OrderBy(x => x.Tag))
                    {
                        CurrentAction.Attack.ActionParameters = keyFrameEvent.ActionLogic.ActionLogic(Player,
                            currentFrame,
                            (currentFrame - currentKeyFrame.StartingFrameIndex),
                            CurrentAction.Attack.ActionParameters);
                    }

                    AdvanceAnimator();
                }
            } else
                _currentMoveSet = null;

            if (Player.player != null)
                State = SetStateLogic(Player.player);
        }

        #endregion Public Methods

        #region Private Methods

        private void AdvanceAnimator()
        {
            _actionAnimator.AdvanceFrame();

            if (!_actionAnimator.InProgress)
                KillAttackSequence();
        }

        private void SetupAnimator()
        {
            HunterCombatMR.Instance.AnimationKeyFrameManager.FillAnimationKeyFrames(_actionAnimator,
                CurrentAction.Attack.FrameProfile);
        }

        private PlayerState SetStateLogic(Player player)
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

        private MoveSet GetOrReturnCurrentMoveSet(string name)
        {
            if (_currentMoveSet != null && name.Equals(_currentMoveSet.InternalName))
                return _currentMoveSet;
            else
                return ContentUtils.GetMoveSet(name);
        }

        #endregion Private Methods
    }
}