using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Items;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Models.Messages.InputSystem;
using HunterCombatMR.Models.Player;
using System.Collections.Generic;
using HunterCombatMR.Extensions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HunterCombatMR.Models.State.Builders;
using HunterCombatMR.Constants;
using HunterCombatMR.Models.State;
using HunterCombatMR.Builders.State;

namespace HunterCombatMR
{
    public class HunterCombatPlayer
        : ModPlayer,
        IAnimationControlled<PlayerAnimationController>
    {
        private WeaponBase _equippedWeapon;
        private IModEntity _entity;

        public HunterCombatPlayer()
            : base()
        {
            StateController = new PlayerStateController(this);
            AnimationController = new PlayerAnimationController();
        }

        public PlayerAnimationController AnimationController { get; }
        public override bool CloneNewInstances => false;
        public bool IsMainPlayer { get => player.whoAmI == Main.myPlayer; }

        public WeaponBase EquippedWeapon
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; }
        }

        public IModEntity EntityReference { get => _entity; }

        public PlayerStateController StateController { get; private set; }

        public override void Initialize()
        {
            _entity = EntityManager.CreateEntity();

            var states = new EntityState[2];
            states[0] = new StateBuilder()
                .WithNewController(StateControllerTypes.ChangeState, 1, new StateTrigger("time = 600"))
                .WithEntityStatuses(EntityWorldStatus.Grounded, EntityActionStatus.Idle)
                .Build();
            states[1] = new StateBuilder()
                .WithNewController(StateControllerTypes.ChangeState, 0, new StateTrigger("time = 600"))
                .WithEntityStatuses(EntityWorldStatus.Grounded, EntityActionStatus.Idle)
                .Build();

            var stateSet = new StateSetBuilder()
                .WithState(StateNumberConstants.Default, states[0])
                .WithState(1, states[1])
                .Build();

            ComponentManager.RegisterComponent(new EntityStateComponent(stateSet), _entity);
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            AnimationController.OnionSkinLogic(ref drawInfo);
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            AnimationController.DrawPlayerLayers(layers);
        }

        public override void OnEnterWorld(Player player)
        {
            StateController.State = EntityWorldStatus.Grounded;
            if (IsMainPlayer && Main.netMode == NetmodeID.SinglePlayer)
            {
                ComponentManager.RegisterComponent(new InputComponent(player.whoAmI), _entity);
            }
        }

        public override void OnRespawn(Player player)
        {
            StateController.State = EntityWorldStatus.Grounded;
            SystemManager.SendMessage(new InputResetMessage(_entity.Id));
        }

        public override void PostSavePlayer()
        {
            if (Main.gameMenu)
            {
                AnimationController.CurrentAnimation = null;
            }

            base.PostSavePlayer();
        }

        public override void PostUpdate()
        {
            if (Main.gameMenu)
                return;

            StateController.StateUpdate();
            StateController.EditorUpdate();
            AnimationController.Animator.Update();
        }

        public override void PostUpdateRunSpeeds()
        {
            MountUpdate();
        }

        public override bool PreItemCheck()
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) || EquippedWeapon != null)
            {
                if (player.itemTime > 0)
                    player.itemTime = 0;
                if (player.itemAnimation > 0)
                    player.itemAnimation = 0;

                return false;
            }

            return base.PreItemCheck();
        }

        public override void PreUpdateMovement()
        {
            StateController.MovementUpdate();
        }

        public override void UpdateDead()
        {
            StateController.State = EntityWorldStatus.Dead;

            SystemManager.SendMessage(new InputResetMessage(_entity.Id));
        }

        private void MountUpdate()
        {
            if (StateController.CurrentAction == null)
                return;

            if (player.mount.Active)
                player.mount.Dismount(player);
        }
    }
}