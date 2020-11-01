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

        public bool ShowLayersDebug { get; set; }

        public LayeredAnimatedAction CurrentAnimation { get; private set; }

        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBufferInfo = new PlayerBufferInformation();
            State = PlayerState.Standing;
            ShowLayersDebug = true;
            LayerPositions = new Dictionary<string, Vector2>();
        }

        public override TagCompound Save()
        {
            ShowLayersDebug = true;
            return base.Save();
        }

        public override void OnEnterWorld(Player player)
        {
            State = PlayerState.Standing;
            ShowLayersDebug = true;

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
            if (!HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                string[] propertiesToChange = new string[] {"hairColor", "eyeWhiteColor", "eyeColor",
                    "faceColor", "bodyColor", "legColor", "shirtColor", "underShirtColor",
                    "pantsColor", "shoeColor", "upperArmorColor", "middleArmorColor",
                    "lowerArmorColor" };

                var properties = drawInfo.GetType().GetFields();

                object temp = drawInfo;

                foreach (var prop in properties.Where(x => propertiesToChange.Contains(x.Name)))
                {
                    if (prop.Name != "hairColor")
                        prop.SetValue(temp, MakeTransparent((Color)prop.GetValue(temp), 30));
                    else
                        prop.SetValue(temp, MakeTransparent((Color)prop.GetValue(temp), 0));
                }

                drawInfo = (PlayerDrawInfo)temp;
            }
        }

        private Color MakeTransparent(Color original,
            byte amount)
        {
            var newColor = original;
            newColor.A = amount;
            return newColor;
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (!HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None) && CurrentAnimation != null)
            {
                var animLayers = CurrentAnimation.LayerData.Layers;
                if (CurrentAnimation.Animation.IsInitialized)
                {
                    var currentFrame = CurrentAnimation.Animation.GetCurrentKeyFrameIndex();

                    foreach (var layer in animLayers.Where(f => f.Frames.ContainsKey(currentFrame)).OrderByDescending(x => x.Frames[currentFrame].LayerDepth))
                    {
                        var newLayer = new PlayerLayer(HunterCombatMR.ModName, layer.Name, delegate (PlayerDrawInfo drawInfo)
                        {
                            CombatLimbDraw(drawInfo, layer.Name, CreateTextureString(layer.Name), layer.GetCurrentFrameRectangle(currentFrame), layer.Frames[currentFrame], currentFrame);
                        });
                        layers.Add(newLayer);
                    }

                    CurrentAnimation.Update();
                }
            }
            else
            {
                CurrentAnimation = null;
                HunterCombatMR.EditorInstance.HighlightedLayers.Clear();
                HunterCombatMR.EditorInstance.SelectedLayer = "";
            }
        }

        private string CreateTextureString(string layerName)
            => $"{_texturePath}{layerName.Split('_')[1]}{_textureSuffix}";

        public static void CombatLimbDraw(PlayerDrawInfo drawInfo,
            string layerName,
            string texturePath,
            Rectangle frameRectangle,
            LayerFrameInfo frameInfo,
            int currentFrame)
        {
            var drawPlayer = drawInfo.drawPlayer;
            var positionVector = new Vector2(drawInfo.position.X - Main.screenPosition.X,
                        drawInfo.position.Y - Main.screenPosition.Y);
            var positions = drawPlayer.GetModPlayer<HunterCombatPlayer>().LayerPositions;

            if (!positions.ContainsKey($"{layerName}-{currentFrame}"))
                positions.Add($"{layerName}-{currentFrame}", new Vector2());

            frameRectangle.SetSheetPositionFromFrame(frameInfo.SpriteFrame);
            DrawData value = new DrawData(ModContent.GetTexture(texturePath), positionVector + frameInfo.Position + positions.FirstOrDefault(x => x.Key.Equals($"{layerName}-{currentFrame}")).Value, frameRectangle, Color.White);

            value = value.SetSpriteOrientation(drawPlayer, frameInfo.SpriteOrientation);

            if (HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.EditMode))
                value = HunterCombatMR.EditorInstance.AdjustPositionLogic(value, positions, $"{layerName}-{currentFrame}");
            
            Main.playerDrawData.Add(value);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!HunterCombatMR.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                player.frozen = true;
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
            base.PostUpdate();
        }

        public bool SetCurrentAnimation(LayeredAnimatedAction newAnimation)
        {
            CurrentAnimation = new LayeredAnimatedAction(newAnimation);

            if (CurrentAnimation != null)
                return true;
            else
                return false;
        }
    }
}