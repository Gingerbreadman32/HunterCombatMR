﻿using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HunterCombatMR.AnimationEngine.Models
{
    public class PlayerActionAnimation
        : Animation,
        IPlayerAnimation,
        IEquatable<Animation>
    {
        #region Public Constructors

        [JsonConstructor]
        public PlayerActionAnimation(string name,
            LayerData layerData,
            bool isInternal)
        {
            Name = name;
            AnimationData = new AnimatedData();
            LayerData = layerData;
            IsInternal = isInternal;
        }

        public PlayerActionAnimation(Animation copy,
            bool newFile = false)
        {
            Name = copy.Name;
            AnimationData = new AnimatedData(copy.AnimationData);
            HunterCombatMR.Instance.AnimationKeyFrameManager.SyncFrames(AnimationData);
            LayerData = new LayerData(copy.LayerData);
            _modified = newFile;
            IsInternal = copy.IsInternal;
        }

        #endregion Public Constructors

        #region Public Properties

        public override AnimationType AnimationType => AnimationType.Player;

        #endregion Public Properties

        #region Public Methods

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

        public List<PlayerLayer> DrawPlayerLayers(List<PlayerLayer> layers)
        {
            List<PlayerLayer> animLayers = layers;
            if (IsAnimationInitialized())
            {
                var currentFrame = AnimationData.GetCurrentKeyFrameIndex();

                foreach (var layer in LayerData.Layers.Where(f => f.KeyFrames.ContainsKey(currentFrame) && f.KeyFrames[currentFrame].IsEnabled).OrderByDescending(x => x.KeyFrames[currentFrame].LayerDepth))
                {
                    var newLayer = new PlayerLayer(HunterCombatMR.ModName, layer.Name, delegate (PlayerDrawInfo drawInfo)
                    {
                        Main.playerDrawData.Add(CombatLimbDraw(drawInfo, layer.Texture, layer.GetCurrentFrameRectangle(currentFrame), layer.KeyFrames[currentFrame], Color.White));
                    });
                    animLayers.Add(newLayer);
                }
            }

            return animLayers;
        }

        public override Animation Duplicate(string name)
            => new PlayerActionAnimation(this) { Name = name, _modified = true };

        public bool Equals(Animation other)
        {
            if (other?.Name == null || other?.LayerData == null || other?.AnimationType == null)
                return false;

            return Name.Equals(other.Name)
                && LayerData.Equals(other.LayerData)
                && AnimationType.Equals(other.AnimationType);
        }

        #endregion Public Methods
    }
}