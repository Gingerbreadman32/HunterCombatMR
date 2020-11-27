using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HunterCombatMR
{
    public class HunterCombatPlayer
        : ModPlayer
    {
        private const bool _bufferText = false;
        private const string _texturePath = "HunterCombatMR/Textures/SnS/";
        private const string _textureSuffix = "Frames_LMB1";

        public PlayerState State { get; set; }

        public ICollection<string> ActiveProjectiles { get; set; }

        public IDictionary<string, Vector2> LayerPositions { get; set; }

        public PlayerBufferInformation InputBufferInfo { get; set; }

        public ActionAnimation CurrentAnimation { get; private set; }

        public bool ShowDefaultLayers { get; private set; }

        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBufferInfo = new PlayerBufferInformation();
            State = PlayerState.Standing;
            LayerPositions = new Dictionary<string, Vector2>();
            ShowDefaultLayers = true;
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

        public override bool PreItemCheck()
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None)) {
                player.itemTime = 0;
                return false;
            }
            else
            {
                return base.PreItemCheck();
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

        private Color MakeTransparent(Color original,
            byte amount)
        {
            var newColor = original;
            newColor.A = amount;
            return newColor;
        }

        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            base.ModifyDrawHeadLayers(layers);
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && CurrentAnimation != null)
            {
                if (!ShowDefaultLayers)
                {
                    foreach (PlayerLayer item in layers)
                    {
                        item.visible = false;
                    }
                }

                var animLayers = CurrentAnimation.LayerData.Layers;
                if (CurrentAnimation.AnimationData.IsInitialized)
                {
                    var currentFrame = CurrentAnimation.AnimationData.GetCurrentKeyFrameIndex();

                    foreach (var layer in animLayers.Where(f => f.Frames.ContainsKey(currentFrame)).OrderByDescending(x => x.Frames[currentFrame].LayerDepth))
                    {
                        var newLayer = new PlayerLayer(HunterCombatMR.ModName, layer.Name, delegate (PlayerDrawInfo drawInfo)
                        {
                            Main.playerDrawData.Add(CombatLimbDraw(drawInfo, CreateTextureString(layer.Name), layer.GetCurrentFrameRectangle(currentFrame), layer.Frames[currentFrame], Color.White));
                        });
                        layers.Add(newLayer);
                    }

                    CurrentAnimation.Update();
                }
            }
        }

        internal static string CreateTextureString(string layerName)
            => $"{_texturePath}{layerName.Split('_')[1]}{_textureSuffix}";

        public static DrawData CombatLimbDraw(PlayerDrawInfo drawInfo,
            string texturePath,
            Rectangle frameRectangle,
            LayerFrameInfo frameInfo,
            Color color)
        {
            var drawPlayer = drawInfo.drawPlayer;

            var positionVector = new Vector2(drawInfo.position.X + (drawPlayer.width / 2) - Main.screenPosition.X,
                        drawInfo.position.Y - Main.screenPosition.Y);

            frameRectangle.SetSheetPositionFromFrame(frameInfo.SpriteFrame);
            DrawData value = new DrawData(ModContent.GetTexture(texturePath), frameInfo.Position, frameRectangle, color);

            value = value.SetSpriteOrientation(drawPlayer, frameInfo, frameRectangle);
            value.position += (positionVector - frameRectangle.Size() / 2);

            return value;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                player.frozen = true;
                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime = 2;
            }
            InputBufferInfo.Update();
            if (_bufferText && InputBufferInfo.BufferedComboInputs.Any(x => x.Input.Equals(ComboInputs.StandardAttack) && x.FramesSinceBuffered == 0))
            {
                CombatText.NewText(new Rectangle((int)Main.player[player.whoAmI].Top.X, (int)Main.player[player.whoAmI].Top.Y, 40, 20), Color.White, "Buffered!");
            }
        }

        public override void UpdateDead()
        {
            State = PlayerState.Dead;
            ActiveProjectiles.Clear();
            InputBufferInfo.ResetBuffers();
        }

        public override void OnRespawn(Player player)
        {
            State = PlayerState.Standing;
            InputBufferInfo.ResetBuffers();
        }

        public override void PostUpdate()
        {
            if (HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode) && CurrentAnimation != null)
            {
                HunterCombatMR.Instance.EditorInstance.AdjustPositionLogic(CurrentAnimation, player.direction);
            }

            base.PostUpdate();
        }

        public bool SetCurrentAnimation(ActionAnimation newAnimation)
        {
            ActionAnimation newAnim = new ActionAnimation(newAnimation);
            CurrentAnimation = newAnim;

            return CurrentAnimation != null;
        }
    }
}