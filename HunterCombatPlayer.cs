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
        private bool _showDefaultLayers = true;

        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBuffers = new PlayerBufferInformation();
            LayerPositions = new Dictionary<string, Vector2>();
            StateController = new PlayerStateController(this);
            AnimationController = new PlayerAnimationController();
        }

        public ICollection<string> ActiveProjectiles { get; set; }
        public bool ActuallyInWorld { get; private set; }
        public override bool CloneNewInstances => false;
        public PlayerAnimationController AnimationController { get; }

        public WeaponBase EquippedWeapon
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; InputBuffers?.ResetBuffers(); }
        }

        public PlayerBufferInformation InputBuffers { get; }
        public IDictionary<string, Vector2> LayerPositions { get; set; }
        public PlayerStateController StateController { get; private set; }

        PlayerAnimationController IAnimationControlled<PlayerAnimationController>.AnimationController => throw new NotImplementedException();

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
                return;

            /* Gotta just rework this, broken for now
            if (AnimationController.CurrentAnimation != null && AnimationController.Animator.CurrentKeyFrameIndex > 0)
            {
                _showDefaultLayers = !HunterCombatMR.Instance.EditorInstance.DrawOnionSkin(drawInfo,
                        AnimationController.Animator,
                        AnimationController.Animator.CurrentKeyFrameIndex - 1,
                        Color.White);
            }
            */

            if (!_showDefaultLayers)
                return;

            string[] propertiesToChange = new string[] {"hairColor", "eyeWhiteColor", "eyeColor",
                    "faceColor", "bodyColor", "legColor", "shirtColor", "underShirtColor",
                    "pantsColor", "shoeColor", "upperArmorColor", "middleArmorColor",
                    "lowerArmorColor" };

            var properties = drawInfo.GetType().GetFields();

            object temp = drawInfo;

            // @@warn cache this, probably shouldn't be using reflection like this every frame
            foreach (var prop in properties.Where(x => propertiesToChange.Contains(x.Name)))
            {
                prop.SetValue(temp, MakeTransparent((Color)prop.GetValue(temp), 30));
            }

            drawInfo = (PlayerDrawInfo)temp;
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (AnimationController == null)
                return;

            if (!_showDefaultLayers || HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                foreach (PlayerLayer item in layers)
                {
                    item.visible = false;
                }
            }

            layers.Where(x => x.Name.Contains("MiscEffects")).ToList().ForEach(x => x.visible = false);

            layers = AnimationController.DrawPlayerLayers(layers);
        }

        public override void OnEnterWorld(Player player)
        {
            ActuallyInWorld = true;
            InputBuffers.Initialize();
            StateController.State = PlayerState.Neutral;

            if (player.whoAmI == Main.myPlayer)
                HunterCombatMR.Instance.SetUIPlayer(player.GetModPlayer<HunterCombatPlayer>());

            _showDefaultLayers = true;

            if (InputBuffers == null)
                throw new Exception("Player's input buffer information not initialized!");

            ActiveProjectiles = new List<string>();

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
                _showDefaultLayers = true;
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
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
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

            if (ActiveProjectiles.Any())
                ActiveProjectiles.Clear();

            InputBuffers.ResetBuffers();
        }

        private Color MakeTransparent(Color original,
                                    byte amount)
        {
            var newColor = original;
            newColor.A = amount;
            return newColor;
        }
    }
}