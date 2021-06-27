using HunterCombatMR.Enumerations;
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
        private bool _showDefaultLayers = true;

        public PlayerAnimationController()
        {
            Animator = new Animator();
        }

        public Animator Animator { get; }

        public bool IsAnimationReady { get => (CurrentAnimation != null && Animator.Initialized); }

        public ICustomAnimationV2 CurrentAnimation 
        { 
            get => _currentAnimation; 
            set 
            {
                Animator.Uninitialize();
                _currentAnimation = value;
                if (value != null)
                    Animator.Initialize(_currentAnimation.Layers.FrameData.Values); 
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
            if (!IsAnimationReady)
                return layers;

            if (!_showDefaultLayers || HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
            {
                foreach (PlayerLayer item in layers)
                {
                    item.visible = false;
                }
            }

            layers.Where(x => x.Name.Contains("MiscEffects")).ToList().ForEach(x => x.visible = false);

            var currentFrame = Animator.CurrentKeyFrameIndex;

            foreach (var layer in CurrentAnimation.Layers.GetOrderedActiveLayerData(currentFrame))
            {
                var newLayer = new PlayerLayer(HunterCombatMR.ModName, layer.Layer.ReferenceName, delegate (PlayerDrawInfo drawInfo)
                {
                    Main.playerDrawData.Add(CombatLimbDraw(drawInfo, TextureUtils.GetTextureFromTag(layer.Layer.Tag), layer.Layer.Tag.Size, layer.FrameData, Color.White));
                });
                layers.Add(newLayer);
            }

            return layers;
        }

        internal void OnionSkinLogic(ref PlayerDrawInfo drawInfo)
        {
            /* Gotta just rework this, broken for now
            if (AnimationController.CurrentAnimation != null && AnimationController.Animator.CurrentKeyFrameIndex > 0)
            {
                _showDefaultLayers = !HunterCombatMR.Instance.EditorInstance.DrawOnionSkin(drawInfo,
                        AnimationController.Animator,
                        AnimationController.Animator.CurrentKeyFrameIndex - 1,
                        Color.White);
            }
            */

            if (!_showDefaultLayers || !IsAnimationReady || HunterCombatMR.Instance.EditorInstance.CurrentEditMode.Equals(EditorMode.None))
                return;

            string[] propertiesToChange = new string[] {"hairColor", "eyeWhiteColor", "eyeColor",
                    "faceColor", "bodyColor", "legColor", "shirtColor", "underShirtColor",
                    "pantsColor", "shoeColor", "upperArmorColor", "middleArmorColor",
                    "lowerArmorColor" };

            var properties = drawInfo.GetType().GetFields();

            object temp = drawInfo;

            // @@warn cache this, probably shouldn't be using reflection like this every frame
            foreach (var prop in properties.Where(x => propertiesToChange.Contains(x.Name)))
            {
                prop.SetValue(temp, MakeTransparent((Color)prop.GetValue(temp), 30));
            }

            drawInfo = (PlayerDrawInfo)temp;
        }

        private Color MakeTransparent(Color original,
                                    byte amount)
        {
            var newColor = original;
            newColor.A = amount;
            return newColor;
        }
    }
}