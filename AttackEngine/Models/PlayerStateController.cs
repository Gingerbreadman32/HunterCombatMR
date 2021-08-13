using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Player;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class PlayerStateController
    {
        private readonly Func<Player, bool>[] _vanillaStateEvaluations = new Func<Player, bool>[] 
            { 
                (p) => { return true; },
                (p) => { return true; },
                (p) => p.IsPlayerWalking(),
                (p) => p.IsPlayerAerial(),
                (p) => p.IsPlayerJumping(),
                (p) => p.dead
            };

        private Animator _actionAnimator;
        private ComboAction _currentAction;
        private MoveSet _currentMoveSet;

        public PlayerStateController(HunterCombatPlayer player)
        {
            _actionAnimator = new Animator();
            State = EntityWorldStatus.NoStatus;
            ActionState = EntityActionStatus.Idle;
            Player = player;
            ActionHistory = new SortedList<int, string>();
            MovementInformation = new MovementInfo();
        }

        public SortedList<int, string> ActionHistory { get; }
        public EntityActionStatus ActionState { get; set; }

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

        public FrameIndex CurrentActionKeyFrame { get => _actionAnimator.CurrentKeyFrameIndex; }

        public MoveSet CurrentMoveSet
        {
            get => _currentMoveSet;
            set
            {
                if (value != _currentMoveSet)
                {
                    _currentMoveSet = value;
                };
            }
        }

        public MovementInfo MovementInformation { get; }
        public HunterCombatPlayer Player { get; }
        public EntityWorldStatus State { get; set; }
        public bool StateOverride { get; set; }

        public void StateUpdate()
        {
            // Set Player State
            if (!StateOverride)
                State = SetStateLogic(Player.player);

            // Equipped Weapon/Moveset Checks
            if (Player.EquippedWeapon == null)
            {
                FullStateReset();
                return;
            }

            /* Get the next action if available
            CurrentAction = PlayerActionComboUtils.GetNextAvailableAction(this,
                CurrentMoveSet,
                Player.InputBuffers);
            */
            // If no actions, stop animations and return
            if (CurrentAction == null)
            {
                ActionReset();
                return;
            }

            ActionLogic();
        }

        public void MovementUpdate()
        {
            if (CurrentAction != null)
                Player.player.velocity = MovementInformation.CalculateVelocity(Player.player.velocity);
        }

        private void ActionLogic()
        {
            CurrentAction.Attack.ActionLogic(Player, _actionAnimator);

            /* need to get rid of this to test
            CurrentAction.Attack.Animations.TryGetAnimation(CurrentActionKeyFrame, out var currentAnimation);
            Player.SetCurrentAnimation(currentAnimation);
            */
            _actionAnimator.Update();

            if (_actionAnimator.Flags.Equals(AnimatorFlags.Locked))
                ActionReset();
        }

        private void ActionReset()
        {
            CurrentAction = null;
            ActionState = EntityActionStatus.Idle;
        }

        private void FullStateReset()
        {
            CurrentMoveSet = null;
            ActionReset();

            if (ActionHistory.Any())
                ActionHistory.Clear();
        }

        private EntityWorldStatus SetStateLogic(Player player)
        {
            EntityWorldStatus currentState = EntityWorldStatus.Dead;

            for (var s = 0; s < _vanillaStateEvaluations.Length; s++)
            {
                if (_vanillaStateEvaluations[s].Invoke(player))
                    currentState = (EntityWorldStatus)s;
            }

            return currentState;
        }

        private void SetupAnimator()
        {
            if (CurrentAction == null)
            {
                _actionAnimator.Uninitialize();
                return;
            }

            _actionAnimator.Initialize(CurrentAction.Attack.Animations.FrameData);
        }
    }
}