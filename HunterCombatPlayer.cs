using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Items;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Player;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR
{
    public class HunterCombatPlayer
        : ModPlayer,
        IAnimationControlled<PlayerAnimationController>
    {
        private WeaponBase _equippedWeapon;

        public HunterCombatPlayer()
            : base()
        {
            InputBuffers = new PlayerBufferInformation();
            StateController = new PlayerStateController(this);
            AnimationController = new PlayerAnimationController();
        }

        public bool ActuallyInWorld { get; private set; }
        public override bool CloneNewInstances => false;
        public PlayerAnimationController AnimationController { get; }

        public WeaponBase EquippedWeapon
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; InputBuffers?.ResetBuffers(); }
        }

        public PlayerBufferInformation InputBuffers { get; }
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
            ActuallyInWorld = true;
            InputBuffers.Initialize();
            StateController.State = PlayerState.Neutral;

            if (player.whoAmI == Main.myPlayer)
                HunterCombatMR.Instance.SetUIPlayer(player.GetModPlayer<HunterCombatPlayer>());

            if (InputBuffers == null)
                throw new Exception("Player's input buffer information not initialized!");

            InputBuffers.ResetBuffers();
        }

        public override void OnRespawn(Player player)
        {
            StateController.State = PlayerState.Neutral;
            InputBuffers.ResetBuffers();
        }

        public override void PostSavePlayer()
        {
            if (Main.gameMenu)
            {
                ActuallyInWorld = false;
                AnimationController.CurrentAnimation = null;
            }

            base.PostSavePlayer();
        }

        public override void PostUpdate()
        {
            if (Main.gameMenu || !ActuallyInWorld)
                return;

            InputBuffers.Update(StateController.State, player.mouseInterface);
            StateController.StateUpdate();
            StateController.EditorUpdate();
            AnimationController.Animator.Update();
            StateController.PostUpdate();
        }

        public override void PostUpdateRunSpeeds()
        {
            StateController.MountUpdate();
        }

        public override bool PreItemCheck()
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) || EquippedWeapon != null)
            {
                if (player.itemTime > 0)
                    player.itemTime = 0;
                if (player.itemAnimation > 0)
                    player.itemAnimation = 0;
            }

            return base.PreItemCheck();
        }

        public override void PreUpdate()
        {
            StateController.PreUpdate();
        }

        public override void PreUpdateMovement()
        {
            StateController.MovementUpdate();
        }

        public override void UpdateDead()
        {
            if (StateController.State != PlayerState.Dead)
                StateController.State = PlayerState.Dead;

            InputBuffers.ResetBuffers();
        }
    }
}