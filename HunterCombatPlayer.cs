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
        private bool _showDefaultLayers = true;
        private WeaponBase _equippedWeapon;
        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBuffers = new PlayerBufferInformation();
            LayerPositions = new Dictionary<string, Vector2>();
            StateController = new PlayerStateController(this);
        }

        public ICollection<string> ActiveProjectiles { get; set; }
        public override bool CloneNewInstances => false;
        public PlayerAnimation CurrentAnimation { get; private set; }
        public WeaponBase EquippedWeapon 
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; InputBuffers?.ResetBuffers();  }
        }
        public PlayerBufferInformation InputBuffers { get; }
        public IDictionary<string, Vector2> LayerPositions { get; set; }
        public PlayerStateController StateController { get; private set; }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                if (CurrentAnimation != null && CurrentAnimation.AnimationData.CurrentKeyFrameIndex > 0)
                    _showDefaultLayers = !HunterCombatMR.Instance.EditorInstance.DrawOnionSkin(drawInfo,
                            CurrentAnimation.LayerData,
                            CurrentAnimation.AnimationData.CurrentKeyFrameIndex - 1,
                            Color.White);
                else
                    _showDefaultLayers = true;

                if (_showDefaultLayers)
                {
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
            }
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
            StateController.State = PlayerState.Neutral;
            HunterCombatMR.Instance.SetUIPlayer(player.GetModPlayer<HunterCombatPlayer>());
            _showDefaultLayers = true;

            if (ActiveProjectiles != null)
                ActiveProjectiles.Clear();
            else
                throw new Exception("Player's active projectiles not initialized!");

            if (InputBuffers != null)
                InputBuffers?.ResetBuffers();
            else
                throw new Exception("Player's input buffer information not initialized!");
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
                CurrentAnimation = null;
                _showDefaultLayers = true;
            }

            base.PostSavePlayer();
        }

        public override void PostUpdate()
        {
            InputBuffers.Update(StateController.State);

            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                player.frozen = true;
                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime = 2;
            }

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode)
                    && (HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing?.AnimationType.Equals(AnimationType.Player) ?? false))
            {
                SetCurrentAnimation(HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing as PlayerAnimation);
            }

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode) && CurrentAnimation != null)
            {
                HunterCombatMR.Instance.EditorInstance.AdjustPositionLogic(CurrentAnimation, player.direction);
            }

            StateController.Update();
            CurrentAnimation?.Update();
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