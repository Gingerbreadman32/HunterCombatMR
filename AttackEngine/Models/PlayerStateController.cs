using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Utilities;
using System.Collections.Generic;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerStateController
    {
        private Animator _actionAnimator;
        private ComboAction _currentAction;
        private MoveSet _currentMoveSet;

        public PlayerStateController(HunterCombatPlayer player)
        {
            _actionAnimator = new Animator();
            State = PlayerState.Neutral;
            ActionState = AttackState.NotAttacking;
            Player = player;
            ActionHistory = new SortedList<int, string>();
        }

        public SortedList<int, string> ActionHistory { get; private set; }
        public AttackState ActionState { get; set; }

        public ComboAction CurrentAction
        {
            get => _currentAction;
            private set
            {
                if (value != _currentAction)
                {
                    _currentAction = value;
                    SetupAnimator();
                };
            }
        }

        public int CurrentActionFrame { get => _actionAnimator.CurrentFrame; }
        public HunterCombatPlayer Player { get; }
        public PlayerState State { get; set; }

        public void Update()
        {
            State = SetStateLogic(Player.player);

            if (Player.EquippedWeapon == null)
            {
                _currentMoveSet = null;
                return;
            }

            _currentMoveSet = ContentUtils.Get<MoveSet>(Player.EquippedWeapon.MoveSet);

            CurrentAction = PlayerActionComboUtils.GetNextAvailableAction(this,
                _currentMoveSet,
                Player.InputBuffers);

            if (CurrentAction != null)
            {
                CurrentAction.Attack.ActionLogic(Player, _actionAnimator);
                _actionAnimator.Update();
            }
        }

        private PlayerState SetStateLogic(Player player)
        {
            // @@info Switch this up with a dictionary run through so it looks cleaner.
            if (State == PlayerState.Dead || Player.player == null)
                return PlayerState.Dead;

            if (player.IsPlayerJumping())
                return PlayerState.Jumping;

            if (player.IsPlayerAerial())
                return PlayerState.Aerial;

            if (player.IsPlayerWalking())
                return PlayerState.Walking;

            return PlayerState.Neutral;
        }

        private void SetupAnimator()
        {
            if (CurrentAction == null)
            {
                _actionAnimator.Uninitialize();
                return;
            }

            _actionAnimator.Initialize(CurrentAction.Attack.KeyFrameProfile);
        }
    }
}