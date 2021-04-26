using HunterCombatMR.AnimationEngine.Enumerations;
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

        public MoveSet CurrentMoveSet
        {
            get => _currentMoveSet;
            private set
            {
                if (value != _currentMoveSet)
                {
                    _currentMoveSet = value;
                };
            }
        }

        public FrameIndex CurrentActionKeyFrame { get => _actionAnimator.CurrentKeyFrameIndex.ToFIndex(); }
        public HunterCombatPlayer Player { get; }
        public PlayerState State { get; set; }

        public void Update()
        {
            // Set Player State
            State = SetStateLogic(Player.player);

            // Equipped Weapon/Moveset Checks
            if (Player.EquippedWeapon == null)
            {
                FullStateReset();
                return;
            }
            CurrentMoveSet = ContentUtils.GetInstance<MoveSet>(Player.EquippedWeapon.MoveSet);

            // Get the next action if available
            CurrentAction = PlayerActionComboUtils.GetNextAvailableAction(this,
                CurrentMoveSet,
                Player.InputBuffers);

            // If no actions, stop animations and return
            if (CurrentAction == null)
            {
                ActionReset();
                return;
            }

            ActionLogic();
        }

        private void ActionLogic()
        {
            CurrentAction.Attack.ActionLogic(Player, _actionAnimator);

            var currentAnimation = CurrentAction.Attack.Animations.GetAnimationByKeyFrame(CurrentActionKeyFrame);
            Player.SetCurrentAnimation(currentAnimation);

            _actionAnimator.Update();

            if (_actionAnimator.Flags.Equals(AnimatorFlags.Locked))
                CurrentAction = null;
        }

        private void FullStateReset()
        {
            CurrentMoveSet = null;
            _currentAction = null;
            ActionReset();

            if (ActionHistory.Any())
                ActionHistory.Clear();
        }

        private void ActionReset()
        {
            Player.SetCurrentAnimation(null);
            ActionState = AttackState.NotAttacking;
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