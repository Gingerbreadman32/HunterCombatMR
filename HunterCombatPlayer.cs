using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Items;
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
        IAnimatedEntity<PlayerAnimation>
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
        }

        public ICollection<string> ActiveProjectiles { get; set; }
        public bool ActuallyInWorld { get; private set; }
        public override bool CloneNewInstances => false;
        public PlayerAnimation CurrentAnimation { get; private set; }

        public WeaponBase EquippedWeapon
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; InputBuffers?.ResetBuffers(); }
        }

        public PlayerBufferInformation InputBuffers { get; }
        public IDictionary<string, Vector2> LayerPositions { get; set; }
        public PlayerStateController StateController { get; private set; }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                return;
            }
            if (CurrentAnimation != null && CurrentAnimation.AnimationData.CurrentKeyFrameIndex > 0)
            {
                _showDefaultLayers = !HunterCombatMR.Instance.EditorInstance.DrawOnionSkin(drawInfo,
                        CurrentAnimation.LayerData,
                        CurrentAnimation.AnimationData.CurrentKeyFrameIndex - 1,
                        Color.White);
            }

            if (!_showDefaultLayers)
            {
                return;
            }

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
            if (CurrentAnimation == null)
            {
                return;
            }

            if (!_showDefaultLayers || HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                foreach (PlayerLayer item in layers)
                {
                    item.visible = false;
                }
            }

            layers.Where(x => x.Name.Contains("MiscEffects")).ToList().ForEach(x => x.visible = false);

            layers = CurrentAnimation.DrawPlayerLayers(layers);
        }

        public override void OnEnterWorld(Player player)
        {
            ActuallyInWorld = true;
            InputBuffers.Initialize();
            StateController.State = PlayerState.Neutral;

            if (player.whoAmI == Main.myPlayer)
                HunterCombatMR.Instance.SetUIPlayer(player.GetModPlayer<HunterCombatPlayer>());

            _showDefaultLayers = true;

            if (ActiveProjectiles == null)
                throw new Exception("Player's active projectiles not initialized!");

            if (InputBuffers == null)
                throw new Exception("Player's input buffer information not initialized!");

            ActiveProjectiles.Clear();

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
                CurrentAnimation = null;
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
            CurrentAnimation?.Update();
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

        public bool SetCurrentAnimation(IAnimation newAnimation,
            bool newFile = false)
        {
            if (newAnimation == null)
            {
                CurrentAnimation?.Uninitialize();
                CurrentAnimation = null;
                return true;
            }

            if (!(newAnimation is PlayerAnimation))
                return false;

            PlayerAnimation newPlayerAnimation = newAnimation as PlayerAnimation;

            if (newPlayerAnimation == CurrentAnimation)
                return true;

            //PlayerActionAnimation newAnim = new PlayerActionAnimation(newAnimation, newFile);
            CurrentAnimation?.Uninitialize();
            CurrentAnimation = newPlayerAnimation;
            CurrentAnimation?.Initialize();

            return CurrentAnimation != null;
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