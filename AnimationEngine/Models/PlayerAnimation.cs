﻿using HunterCombatMR.AnimationEngine.Extensions;
using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class PlayerAnimation
        : Animation,
        IPlayerAnimation,
        IHunterCombatContentInstance
    {
        [JsonConstructor]
        public PlayerAnimation(string name,
            LayerData layerData,
            bool isInternal)
            : base(name)
        {
            Name = name;
            AnimationData = new Animator();
            LayerData = layerData;
            IsStoredInternally = isInternal;
        }

        public PlayerAnimation(PlayerAnimation copy,
            string name)
            : base(name)
        {
            Name = copy.Name;
            LayerData = new LayerData(copy.LayerData);
            IsStoredInternally = copy.IsStoredInternally;
            AnimationData = new Animator();
            Initialize();
        }

        public override AnimationType AnimationType { get => AnimationType.Player; }

        public static DrawData CombatLimbDraw(PlayerDrawInfo drawInfo,
            Texture2D texture,
            Rectangle frameRectangle,
            LayerFrameInfo frameInfo,
            Color color)
        {
            var drawPlayer = drawInfo.drawPlayer;

            var positionVector = new Vector2(drawInfo.position.X + (drawPlayer.width / 2) - Main.screenPosition.X,
                        drawInfo.position.Y - Main.screenPosition.Y);

            frameRectangle.SetSheetPositionFromFrame(frameInfo.SpriteFrame);
            DrawData value = new DrawData(texture, frameInfo.Position, frameRectangle, color);

            value = value.SetSpriteOrientation(drawPlayer, frameInfo, frameRectangle);
            value.position += (positionVector - frameRectangle.Size() / 2);

            return value;
        }

        public override IHunterCombatContentInstance CloneFrom(string internalName)
            => new PlayerAnimation(this, internalName);

        public List<PlayerLayer> DrawPlayerLayers(List<PlayerLayer> layers)
        {
            List<PlayerLayer> animLayers = layers;
            if (IsInitialized)
            {
                var currentFrame = AnimationData.CurrentKeyFrameIndex;

                foreach (var layer in LayerData.Layers.Where(f => f.KeyFrames.ContainsKey(currentFrame) && f.KeyFrames[currentFrame].IsEnabled).OrderByDescending(x => x.KeyFrames[currentFrame].LayerDepth))
                {
                    var newLayer = new PlayerLayer(HunterCombatMR.ModName, layer.Name, delegate (PlayerDrawInfo drawInfo)
                    {
                        Main.playerDrawData.Add(CombatLimbDraw(drawInfo, layer.Texture, layer.GetFrameRectangle(currentFrame), layer.KeyFrames[currentFrame], Color.White));
                    });
                    animLayers.Add(newLayer);
                }
            }

            return animLayers;
        }
    }
}