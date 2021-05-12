using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
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
            State = PlayerState.Neutral;
            ActionState = AttackState.NotAttacking;
            Player = player;
            ActionHistory = new SortedList<int, string>();
            MovementInformation = new MovementInfo();
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
        public PlayerState State { get; set; }
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

        public void PostUpdate()
        {
            Main.blockInput = HunterCombatMR.Instance.VanillaBlockInput;
        }

        public void MountUpdate()
        {
            if (CurrentAction == null)
                return;

            if (Player.player.mount.Active)
                Player.player.mount.Dismount(Player.player);
        }

        public void EditorUpdate()
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                if (Main.blockInput)
                    Main.blockInput = false;
                return;
            }
                Main.blockInput = true;
                Player.player.immune = true;
                Player.player.immuneNoBlink = true;
                Player.player.immuneTime = 2;

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.AnimationEdit)
                    && (HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing?.AnimationType.Equals(AnimationType.Player) ?? false))
            {
                Player.SetCurrentAnimation(HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing);
            }

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.AnimationEdit) 
                && Player.CurrentAnimation != null)
            {
                HunterCombatMR.Instance.EditorInstance.AdjustPositionLogic(Player.CurrentAnimation, Player.player.direction);
            }
        }

        public void PreUpdate()
        {
            HunterCombatMR.Instance.VanillaBlockInput = Main.blockInput;

            if (CurrentAction == null)
                return;

            Main.blockInput = true;
        }

        public void MovementUpdate()
        {
            if (CurrentAction != null)
                Player.player.velocity = MovementInformation.CalculateVelocity(Player.player.velocity);
        }

        private void ActionLogic()
        {
            CurrentAction.Attack.ActionLogic(Player, _actionAnimator);

            var currentAnimation = CurrentAction.Attack.Animations.GetAnimationByKeyFrame(CurrentActionKeyFrame);
            Player.SetCurrentAnimation(currentAnimation);

            _actionAnimator.Update();

            if (_actionAnimator.Flags.Equals(AnimatorFlags.Locked))
                ActionReset();
        }

        private void ActionReset()
        {
            CurrentAction = null;
            Player.SetCurrentAnimation(null);
            ActionState = AttackState.NotAttacking;
            Main.blockInput = false;
        }

        private void FullStateReset()
        {
            CurrentMoveSet = null;
            ActionReset();

            if (ActionHistory.Any())
                ActionHistory.Clear();
        }

        private PlayerState SetStateLogic(Player player)
        {
            PlayerState currentState = PlayerState.Dead;

            for (var s = 0; s < _vanillaStateEvaluations.Length; s++)
            {
                if (_vanillaStateEvaluations[s].Invoke(player))
                    currentState = (PlayerState)s;
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

            _actionAnimator.Initialize(CurrentAction.Attack.KeyFrameProfile);
        }
    }
}