using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
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
        : Animation<HunterCombatPlayer, PlayerActionAnimation>,
        IPlayerAnimation,
        IHunterCombatContentInstance
    {
        #region Public Constructors

        [JsonConstructor]
        public PlayerActionAnimation(string name,
            LayerData layerData,
            bool isInternal)
            : base(name)
        {
            Name = name;
            AnimationData = new Animator<PlayerActionAnimation, HunterCombatPlayer, PlayerActionAnimation>();
            LayerData = layerData;
            IsInternal = isInternal;
        }

        public PlayerActionAnimation(PlayerActionAnimation copy,
            bool newFile = false)
            : base(copy.InternalName)
        {
            Name = copy.Name;
            AnimationData = new Animator<PlayerActionAnimation, HunterCombatPlayer, PlayerActionAnimation>(copy.AnimationData);
            LayerData = new LayerData(copy.LayerData);
            IsModified = newFile;
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
                var currentFrame = AnimationData.CurrentKeyFrameIndex;

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

        public override T Duplicate<T>(string name)
        {
            var copy = base.Duplicate<T>(name);
            copy = (T)MemberwiseClone();
            var playerAnim = copy as PlayerActionAnimation;
            playerAnim.IsModified = true;
            playerAnim.Initialize();

            return copy;
        }

        #endregion Public Methods
    }
}