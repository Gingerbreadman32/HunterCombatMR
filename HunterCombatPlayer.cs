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
        : ModPlayer
    {
        #region Private Fields

        private const bool _bufferText = false;

        #endregion Private Fields

        #region Public Constructors

        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBufferInfo = new PlayerBufferInformation();
            State = PlayerState.Standing;
            LayerPositions = new Dictionary<string, Vector2>();
            ShowDefaultLayers = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public ICollection<string> ActiveProjectiles { get; set; }
        public PlayerActionAnimation CurrentAnimation { get; private set; }
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
            State = PlayerState.Standing;
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
            State = PlayerState.Standing;
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

            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode) && CurrentAnimation != null)
            {
                HunterCombatMR.Instance.EditorInstance.AdjustPositionLogic(CurrentAnimation, player.direction);
            }

            base.PostUpdate();
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
            if (_bufferText && InputBufferInfo.BufferedComboInputs.Any(x => x.Input.Equals(ComboInputs.StandardAttack) && x.FramesSinceBuffered == 0))
            {
                CombatText.NewText(new Rectangle((int)Main.player[player.whoAmI].Top.X, (int)Main.player[player.whoAmI].Top.Y, 40, 20), Color.White, "Buffered!");
            }
        }

        public bool SetCurrentAnimation(AnimationEngine.Models.Animation newAnimation,
            bool newFile = false)
        {
            if (newAnimation == null)
            {
                CurrentAnimation = null;
                return true;
            }

            PlayerActionAnimation newAnim = new PlayerActionAnimation(newAnimation, newFile);
            CurrentAnimation = newAnim;

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

        #endregion Private Methods
    }
}