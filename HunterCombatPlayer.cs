using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
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
        IAnimated<PlayerActionAnimation>
    {
        #region Private Fields

        private bool _showDefaultLayers;

        #endregion Private Fields

        #region Public Constructors

        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBuffers = new PlayerBufferInformation();
            LayerPositions = new Dictionary<string, Vector2>();
            _showDefaultLayers = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public ICollection<string> ActiveProjectiles { get; set; }
        public PlayerActionAnimation CurrentAnimation { get; private set; }
        public PlayerBufferInformation InputBuffers { get; set; }

        // Instead of using this, move the "pop-ups" to their own layer in the normal editor uistate
        public IDictionary<string, Vector2> LayerPositions { get; set; }

        public PlayerStateController StateController { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                if (CurrentAnimation != null && CurrentAnimation.AnimationData.GetCurrentKeyFrameIndex() > 0)
                    _showDefaultLayers = !HunterCombatMR.Instance.EditorInstance.DrawOnionSkin(drawInfo,
                            CurrentAnimation.LayerData,
                            CurrentAnimation.AnimationData.GetCurrentKeyFrameIndex() - 1,
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
                } else
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
            StateController = new PlayerStateController(this);

            if (ActiveProjectiles != null)
                ActiveProjectiles.Clear();
            else
                throw new Exception("Player's active projectiles not initialized!");

            if (InputBuffers != null)
                InputBuffers.ResetBuffers();
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
                StateController = null;
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
                SetCurrentAnimation(HunterCombatMR.Instance.EditorInstance.CurrentAnimationEditing as PlayerActionAnimation);
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
            if (!StateController.State.Equals(PlayerState.Dead))
                InputBuffers.Update();
        }

        public bool SetCurrentAnimation(PlayerActionAnimation newAnimation,
            bool newFile = false)
        {
            if (newAnimation == null)
            {
                CurrentAnimation = null;
                return true;
            }

            if (newAnimation == CurrentAnimation)
                return true;

            //PlayerActionAnimation newAnim = new PlayerActionAnimation(newAnimation, newFile);
            CurrentAnimation = newAnimation;

            return CurrentAnimation != null;
        }

        public override void UpdateDead()
        {
            StateController.State = PlayerState.Dead;

            if(ActiveProjectiles.Any())
                ActiveProjectiles.Clear();

            if (InputBuffers.BufferedComboInputs.Any() || InputBuffers.HeldComboInputs.Any(x => x.Value > 0))
                InputBuffers.ResetBuffers();
        }

        #endregion Public Methods

        #region Private Methods

        private Color MakeTransparent(Color original,
                                    byte amount)
        {
            var newColor = original;
            newColor.A = amount;
            return newColor;
        }

        #endregion Private Methods
    }
}