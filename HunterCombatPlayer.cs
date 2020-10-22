using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public PlayerAttackState AttackState { get; set; }

        public ICollection<string> ActiveProjectiles { get; set; }

        public IDictionary<string, Vector2> LayerPositions { get; set; }

        public PlayerBufferInformation InputBufferInfo { get; set; }

        public bool ShowLayersDebug { get; set; }

        public HunterCombatPlayer()
            : base()
        {
            ActiveProjectiles = new List<string>();
            InputBufferInfo = new PlayerBufferInformation();
            AttackState = PlayerAttackState.NotAttacking;
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
            AttackState = PlayerAttackState.NotAttacking;
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
            if (!HunterCombatMR.EditMode.Equals(EditorMode.None))
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
            Dictionary<string, Rectangle> Layers = new Dictionary<string, Rectangle>() { { "HC_BackLeg", new Rectangle(0, 0, 18, 16) },
                { "HC_BackArm", new Rectangle(0, 0, 32, 26) },
                { "HC_Body", new Rectangle(0, 0, 26, 18) },
                { "HC_FrontLeg", new Rectangle(0, 0, 22, 14) },
                { "HC_Head", new Rectangle(0, 0, 16, 18) },
                { "HC_FrontArm", new Rectangle(0, 0, 18, 14) } };

            foreach (var layer in Layers)
            {
                layers.Add(new PlayerLayer(HunterCombatMR.ModName, layer.Key, delegate (PlayerDrawInfo drawInfo)
                {
                    CombatLimbDraw(drawInfo, layer.Key, CreateTextureString(layer.Key), layer.Value);
                })
                { visible = false });
            }

            if (!HunterCombatMR.EditMode.Equals(EditorMode.None))
            {
                //layers.ForEach((x) => { x.visible = false; });
                //layers.Where(x => x.Name.ToLower().Contains("hair")).ToList().ForEach((x) => { x.visible = false; });
                layers.Where(x => x.Name.Contains("HC_")).ToList().ForEach((x) => { x.visible = true; });
            }
        }

        private string CreateTextureString(string layerName)
            => $"{_texturePath}{layerName.Split('_')[1]}{_textureSuffix}";

        public static void CombatLimbDraw(PlayerDrawInfo drawInfo,
            string layerName,
            string texturePath,
            Rectangle frameRectangle)
        {
            var drawPlayer = drawInfo.drawPlayer;
            var positions = drawPlayer.GetModPlayer<HunterCombatPlayer>().LayerPositions;

            if (!positions.ContainsKey(layerName))
                positions.Add(layerName,
                    new Vector2(drawPlayer.headPosition.X + drawInfo.position.X - Main.screenPosition.X + (float)(drawPlayer.width / 2),
                        drawPlayer.headPosition.Y + drawInfo.position.Y - Main.screenPosition.Y + (float)(drawPlayer.height / 2)));

            var value = new DrawData(ModContent.GetTexture(texturePath), positions[layerName], frameRectangle, Color.White);
            value.effect = (drawPlayer.direction == 1) ? Microsoft.Xna.Framework.Graphics.SpriteEffects.None : Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;

            if (HunterCombatMR.EditMode.Equals(EditorMode.EditMode))
                value = AdjustPositionLogic(value, positions, layerName);

            Main.playerDrawData.Add(value);
        }

        public static DrawData AdjustPositionLogic(DrawData drawData,
            IDictionary<string, Vector2> positions,
            string layerName)
        {
            var value = drawData;
            var MousePosition = new Vector2(Main.mouseX, Main.mouseY);
            var texPosition = new Rectangle((int)positions[layerName].X, (int)positions[layerName].Y, value.sourceRect.GetValueOrDefault().Width, value.sourceRect.GetValueOrDefault().Height);
            if (texPosition.Contains(MousePosition.ToPoint()))
            {
                if (!HunterCombatMR.HighlightedLayers.Contains(layerName))
                    HunterCombatMR.HighlightedLayers.Add(layerName);

                value.color = Color.Red;

                if (PlayerInput.MouseInfoOld.LeftButton.Equals(ButtonState.Pressed) && Main.mouseLeft &&
                    HunterCombatMR.HighlightedLayers.Last().Equals(layerName) && HunterCombatMR.SelectedLayer == "")
                {
                    HunterCombatMR.SelectedLayer = layerName;
                }
            }
            else
            {
                if (HunterCombatMR.HighlightedLayers.Contains(layerName))
                    HunterCombatMR.HighlightedLayers.Remove(layerName);
            }

            if (HunterCombatMR.SelectedLayer == layerName)
            {
                value.color = Color.Green;
                var dragPosition = new Vector2(MousePosition.X - (value.sourceRect.GetValueOrDefault().Width / 2), MousePosition.Y - (value.sourceRect.GetValueOrDefault().Height / 2));

                if (HunterCombatMR.SelectedLayerNudgeAmount.HasValue && HunterCombatMR.NudgeCooldown.HasValue)
                {
                    if (HunterCombatMR.NudgeCooldown.Value > 0 && HunterCombatMR.NudgeCooldown < 2)
                    {
                        HunterCombatMR.NudgeCooldown = HunterCombatMR.NudgeCooldown.Value - 1;
                    }
                    else if (HunterCombatMR.NudgeCooldown == 2)
                    {
                        if (PlayerInput.Triggers.JustReleased.Down)
                        {
                            var nudgeAmount = (positions[layerName].Y % 2 != 0) ? 2 : 1;
                            var newNudge = new Point(HunterCombatMR.SelectedLayerNudgeAmount.Value.X, HunterCombatMR.SelectedLayerNudgeAmount.Value.Y + nudgeAmount);
                            HunterCombatMR.SelectedLayerNudgeAmount = newNudge;
                            HunterCombatMR.NudgeCooldown = HunterCombatMR.NudgeCooldown.Value - 1;
                        }
                        else if (PlayerInput.Triggers.JustReleased.Up)
                        {
                            var nudgeAmount = (positions[layerName].Y % 2 != 0) ? 2 : 1;
                            var newNudge = new Point(HunterCombatMR.SelectedLayerNudgeAmount.Value.X, HunterCombatMR.SelectedLayerNudgeAmount.Value.Y - nudgeAmount);
                            HunterCombatMR.SelectedLayerNudgeAmount = newNudge;
                            HunterCombatMR.NudgeCooldown = HunterCombatMR.NudgeCooldown.Value - 1;
                        }
                        else if (PlayerInput.Triggers.JustReleased.Left)
                        {
                            var nudgeAmount = (positions[layerName].X % 2 == 0) ? 2 : 1;
                            var newNudge = new Point(HunterCombatMR.SelectedLayerNudgeAmount.Value.X - nudgeAmount, HunterCombatMR.SelectedLayerNudgeAmount.Value.Y);
                            HunterCombatMR.SelectedLayerNudgeAmount = newNudge;
                            HunterCombatMR.NudgeCooldown = HunterCombatMR.NudgeCooldown.Value - 1;
                        }
                        else if (PlayerInput.Triggers.JustReleased.Right)
                        {
                            var nudgeAmount = (positions[layerName].X % 2 == 0) ? 2 : 1;
                            var newNudge = new Point(HunterCombatMR.SelectedLayerNudgeAmount.Value.X + nudgeAmount, HunterCombatMR.SelectedLayerNudgeAmount.Value.Y);
                            HunterCombatMR.SelectedLayerNudgeAmount = newNudge;
                            HunterCombatMR.NudgeCooldown = HunterCombatMR.NudgeCooldown.Value - 1;
                        }
                    }
                    else
                    {
                        HunterCombatMR.NudgeCooldown = 2;
                    }
                }

                positions[layerName] = dragPosition + HunterCombatMR.SelectedLayerNudgeAmount.GetValueOrDefault().ToVector2();
            }

            if (Main.mouseLeftRelease)
            {
                HunterCombatMR.SelectedLayer = "";
                HunterCombatMR.SelectedLayerNudgeAmount = new Point(0, 0);
            }

            return value;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!HunterCombatMR.EditMode.Equals(EditorMode.None))
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
            AttackState = PlayerAttackState.NotAttacking;
            ActiveProjectiles.Clear();
            InputBufferInfo.ResetBuffers();
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
        }
    }
}