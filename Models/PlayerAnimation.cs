using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HunterCombatMR.Models
{
    public class PlayerAnimation
        : CustomAnimation,
        IPlayerAnimation,
        IContent
    {
        [JsonConstructor]
        public PlayerAnimation(string name,
            ExtraAnimationData layerData,
            bool isInternal)
            : base(name)
        {
            DisplayName = name;
            AnimationData = new Animator();
            LayerData = layerData;
            IsStoredInternally = isInternal;
        }

        public PlayerAnimation(PlayerAnimation copy,
            string name)
            : base(name)
        {
            DisplayName = copy.DisplayName;
            LayerData = new ExtraAnimationData(copy.LayerData);
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

            var positionVector = new Vector2(drawInfo.position.X + drawPlayer.width / 2 - Main.screenPosition.X,
                        drawInfo.position.Y - Main.screenPosition.Y);

            frameRectangle.SetSheetPositionFromFrame(frameInfo.SpriteFrame);
            DrawData value = new DrawData(texture, frameInfo.Position, frameRectangle, color);

            value = value.SetSpriteOrientation(drawPlayer, new LayerData(frameInfo), frameRectangle);
            value.position += positionVector - frameRectangle.Size() / 2;

            return value;
        }

        public override IContent CreateNew(string internalName)
            => new PlayerAnimation(this, internalName);

        public List<PlayerLayer> DrawPlayerLayers(List<PlayerLayer> layers)
        {
            List<PlayerLayer> animLayers = layers;
            if (IsInitialized)
            {
                var currentFrame = AnimationData.CurrentKeyFrameIndex;

                foreach (var layer in LayerData.Layers.Where(f => f.KeyFrames.ContainsKey(currentFrame) && f.KeyFrames[currentFrame].IsEnabled).OrderByDescending(x => x.KeyFrames[currentFrame].LayerDepth))
                {
                    var newLayer = new PlayerLayer(ModConstants.ModName, layer.DisplayName, delegate (PlayerDrawInfo drawInfo)
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