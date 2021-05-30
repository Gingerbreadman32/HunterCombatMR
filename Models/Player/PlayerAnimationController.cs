using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HunterCombatMR.Models.Player
{
    public class PlayerAnimationController
        : IAnimationController
    {
        private ICustomAnimationV2 _currentAnimation;

        public PlayerAnimationController()
        {
            Animator = new Animator();
        }

        public Animator Animator { get; }

        public ICustomAnimationV2 CurrentAnimation 
        { 
            get => _currentAnimation; 
            set 
            {
                Animator.Uninitialize();
                _currentAnimation = value;
                if (value != null)
                    Animator.Initialize(_currentAnimation.Layers.FrameData); 
            } 
        }

        public static DrawData CombatLimbDraw(PlayerDrawInfo drawInfo,
            Texture2D texture,
            Point textureSize,
            LayerData frameInfo,
            Color color)
        {
            var frameRectangle = new Rectangle(0, 0, textureSize.X, textureSize.Y);
            var drawPlayer = drawInfo.drawPlayer;

            var positionVector = new Vector2(drawInfo.position.X + drawPlayer.width / 2 - Main.screenPosition.X,
                        drawInfo.position.Y - Main.screenPosition.Y);

            frameRectangle.SetSheetPositionFromFrame(frameInfo.SheetFrame);
            DrawData value = new DrawData(texture, frameInfo.Position, frameRectangle, color);

            value = value.SetSpriteOrientation(drawPlayer, frameInfo, frameRectangle);
            value.position += positionVector - frameRectangle.Size() / 2;

            return value;
        }

        public List<PlayerLayer> DrawPlayerLayers(List<PlayerLayer> layers)
        {
            List<PlayerLayer> animLayers = layers;

            var currentFrame = Animator.CurrentKeyFrameIndex;

            foreach (var layer in CurrentAnimation.Layers.GetOrderedActiveLayerData(currentFrame))
            {
                var newLayer = new PlayerLayer(HunterCombatMR.ModName, layer.Layer.Name, delegate (PlayerDrawInfo drawInfo)
                {
                    Main.playerDrawData.Add(CombatLimbDraw(drawInfo, TextureUtils.GetTextureFromTag(layer.Layer.Tag), layer.Layer.Tag.Size, layer.FrameData, Color.White));
                });
                animLayers.Add(newLayer);
            }

            return animLayers;
        }
    }
}