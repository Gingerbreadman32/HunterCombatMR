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
using Terraria.GameInput;
using Terraria.ModLoader;

namespace HunterCombatMR
{
    public class HunterCombatPlayer
        : ModPlayer,
        IAnimatedEntity<PlayerAnimation>
    {
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
        public override bool CloneNewInstances => false;
        public PlayerAnimation CurrentAnimation { get; private set; }
        public WeaponBase EquippedWeapon { get; set; }
        public PlayerBufferInformation InputBuffers { get; private set; }
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
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                if (!_showDefaultLayers)
                {
                    foreach (PlayerLayer item in layers)
                    {
                        item.visible = false;
                    }
                }
                else
                    layers.Where(x => x.Name.Contains("MiscEffects")).ToList().ForEach(x => x.visible = false);

                if (CurrentAnimation != null)
                {
                    layers = CurrentAnimation.DrawPlayerLayers(layers);
                    CurrentAnimation.Update();
                }
            }
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

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!StateController.State.Equals(PlayerState.Dead) && EquippedWeapon != null)
                InputBuffers.Update();
        }

        public bool SetCurrentAnimation(IAnimation newAnimation,
            bool newFile = false)
        {
            if (!(newAnimation is PlayerAnimation))
                return false;

            if (newAnimation == null)
            {
                CurrentAnimation = null;
                return true;
            }

            PlayerAnimation newPlayerAnimation = newAnimation as PlayerAnimation;

            if (newPlayerAnimation == CurrentAnimation)
                return true;

            //PlayerActionAnimation newAnim = new PlayerActionAnimation(newAnimation, newFile);
            CurrentAnimation = newPlayerAnimation;

            return CurrentAnimation != null;
        }

        public override void UpdateDead()
        {
            if (StateController.State != PlayerState.Dead)
                StateController.State = PlayerState.Dead;

            if (ActiveProjectiles.Any())
                ActiveProjectiles.Clear();

            if (InputBuffers.BufferedComboInputs.Any() || InputBuffers.HeldComboInputs.Any(x => x.Value > 0))
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