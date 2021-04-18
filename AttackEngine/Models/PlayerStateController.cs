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

        private Animator<PlayerAction, HunterCombatPlayer, PlayerAnimation> _actionAnimator;
        private MoveSet _currentMoveSet;

        #endregion Private Fields

        #region Public Constructors

        public PlayerStateController(HunterCombatPlayer player)
        {
            _actionAnimator = new Animator<PlayerAction, HunterCombatPlayer, PlayerAnimation>();
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

        public void SetToNeutral()
        {
            SetNewAction(null);
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
                    if (!_actionAnimator.InProgress)
                    {
                        SetToNeutral();
                    }

                    CurrentAction.Attack.Update(_actionAnimator);
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
                SetToNeutral();
        }

        private void SetupAnimator()
        {
            _actionAnimator.Initialize(CurrentAction.Attack.KeyFrameProfile);
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
