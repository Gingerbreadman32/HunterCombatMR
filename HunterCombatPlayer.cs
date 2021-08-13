using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Items;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
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
using HunterCombatMR.Messages.InputSystem;
using HunterCombatMR.Messages.EntityStateSystem;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Models.Animation;

namespace HunterCombatMR
{
    public class HunterCombatPlayer
        : ModPlayer
    {
        private WeaponBase _equippedWeapon;
        private IModEntity _entity;

        public HunterCombatPlayer()
            : base()
        {
            StateController = new PlayerStateController(this);
        }

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
                .WithParameters(animation: -1)
                .Build();
            states[1] = new StateBuilder()
                .WithNewController(StateControllerTypes.ChangeState, 0, new StateTrigger("time = 600"))
                .WithEntityStatuses(EntityWorldStatus.Grounded, EntityActionStatus.Idle)
                .WithParameters(animation: 1)
                .Build();

            var stateSet = new StateSetBuilder()
                .WithState(StateNumberConstants.Default, states[0])
                .WithState(1, states[1])
                .Build();

            ComponentManager.RegisterComponent(new EntityStateComponent(stateSet), _entity);
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            _entity.SendMessage(new CreatePlayerLayersMessage(_entity.Id, ref layers));
        }

        public override void OnEnterWorld(Player player)
        {
            _entity.SendMessage(new SetWorldStatusMessage(_entity.Id, EntityWorldStatus.Grounded));
            if (IsMainPlayer && Main.netMode == NetmodeID.SinglePlayer)
            {
                ComponentManager.RegisterComponent(new InputComponent(player.whoAmI), _entity);
            }
        }

        public override void OnRespawn(Player player)
        {
            _entity.SendMessage(new SetWorldStatusMessage(_entity.Id, EntityWorldStatus.Grounded));
            _entity.SendMessage(new InputResetMessage(_entity.Id));
        }

        public override void PostSavePlayer()
        {
            if (Main.gameMenu && _entity.HasComponent<AnimationComponent>())
            {
                _entity.RemoveComponent<AnimationComponent>();
            }

            base.PostSavePlayer();
        }

        public override void PostUpdate()
        {
            if (Main.gameMenu)
                return;

            StateController.StateUpdate();
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
            _entity.SendMessage(new SetWorldStatusMessage(_entity.Id, EntityWorldStatus.Dead));
            _entity.SendMessage(new InputResetMessage(_entity.Id));
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