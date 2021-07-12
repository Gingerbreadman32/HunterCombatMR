using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Items;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Models.Messages.InputSystem;
using HunterCombatMR.Models.Player;
using HunterCombatMR.Services;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR
{
    public class HunterCombatPlayer
        : ModPlayer,
        IAnimationControlled<PlayerAnimationController>,
        IModEntity
    {
        private WeaponBase _equippedWeapon;
        private PlayerStateComponent _stateComponent;

        public HunterCombatPlayer()
            : base()
        {
            StateController = new PlayerStateController(this);
            AnimationController = new PlayerAnimationController();
            _stateComponent = new PlayerStateComponent();
        }

        public PlayerAnimationController AnimationController { get; }
        public override bool CloneNewInstances => false;

        public WeaponBase EquippedWeapon
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; }
        }

        public int Id => player.whoAmI;
        public PlayerStateController StateController { get; private set; }

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
            StateController.State = EntityWorldStatus.Neutral;

            if (player.whoAmI == Main.myPlayer)
                HunterCombatMR.Instance.SetUIPlayer(player.GetModPlayer<HunterCombatPlayer>());
        }

        public override void OnRespawn(Player player)
        {
            StateController.State = EntityWorldStatus.Neutral;
            SystemManager.SendMessage(new InputResetMessage(player.whoAmI));
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

            SystemManager.SendMessage(new InputResetMessage(player.whoAmI));
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