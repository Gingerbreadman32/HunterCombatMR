using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
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

        #region Public Constructors

        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBufferInfo = new PlayerBufferInformation();
            State = PlayerState.Neutral;
            LayerPositions = new Dictionary<string, Vector2>();
            ShowDefaultLayers = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public ICollection<string> ActiveProjectiles { get; set; }
        public PlayerActionAnimation CurrentAnimation { get; private set; }

        public PlayerAttackState AttackState { get; private set; }
        public PlayerBufferInformation InputBufferInfo { get; set; }
        public IDictionary<string, Vector2> LayerPositions { get; set; }
        public bool ShowDefaultLayers { get; private set; }
        public PlayerState State { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            base.ModifyDrawHeadLayers(layers);
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                if (CurrentAnimation != null && CurrentAnimation.AnimationData.GetCurrentKeyFrameIndex() > 0)
                    ShowDefaultLayers = !HunterCombatMR.Instance.EditorInstance.DrawOnionSkin(drawInfo,
                            CurrentAnimation.LayerData,
                            CurrentAnimation.AnimationData.GetCurrentKeyFrameIndex() - 1,
                            Color.White);
                else
                    ShowDefaultLayers = true;

                if (ShowDefaultLayers)
                {
                    string[] propertiesToChange = new string[] {"hairColor", "eyeWhiteColor", "eyeColor",
                    "faceColor", "bodyColor", "legColor", "shirtColor", "underShirtColor",
                    "pantsColor", "shoeColor", "upperArmorColor", "middleArmorColor",
                    "lowerArmorColor" };

                    var properties = drawInfo.GetType().GetFields();

                    object temp = drawInfo;

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
                if (!ShowDefaultLayers)
                {
                    foreach (PlayerLayer item in layers)
                    {
                        item.visible = false;
                    }
                }
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
            State = PlayerState.Neutral;
            HunterCombatMR.Instance.SetUIPlayer(player.GetModPlayer<HunterCombatPlayer>());
            ShowDefaultLayers = true;

            if (ActiveProjectiles != null)
                ActiveProjectiles.Clear();
            else
                throw new Exception("Player's active projectiles not initialized!");

            if (InputBufferInfo != null)
                InputBufferInfo.ResetBuffers();
            else
                throw new Exception("Player's input buffer information not initialized!");
        }

        public override void OnRespawn(Player player)
        {
            State = PlayerState.Neutral;
            InputBufferInfo.ResetBuffers();
        }

        public override void PostSavePlayer()
        {
            if (Main.gameMenu)
            {
                CurrentAnimation = null;
                ShowDefaultLayers = true;
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

            SetState();
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
            InputBufferInfo.Update();
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
            State = PlayerState.Dead;
            ActiveProjectiles.Clear();
            InputBufferInfo.ResetBuffers();
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

        private void SetState()
        {
            if (State == PlayerState.Dead)
                return;

            State = PlayerState.Neutral;

            if (player.IsPlayerWalking())
                State = PlayerState.Walking;

            if (player.IsPlayerAerial())
                State = PlayerState.Aerial;

            if (player.IsPlayerJumping())
                State = PlayerState.Jumping;

        }

        #endregion Private Methods
    }
}