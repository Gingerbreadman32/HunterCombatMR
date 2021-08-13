using HunterCombatMR.Constants;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HunterCombatMR.Systems
{
    public class AnimationSystem
        : ModSystem<AnimationComponent>,
        IMessageHandler<ChangeAnimationMessage>,
        IMessageHandler<CreatePlayerLayersMessage>
    {
        private List<Queue<ChangeAnimationMessage>> _changeMessages;

        public IReadOnlyList<Queue<ChangeAnimationMessage>> AnimationChangeQueue { get => _changeMessages; }

        public static DrawData SetDrawLayer(PlayerDrawInfo drawInfo,
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

        public void CreatePlayerLayersForAnimation(ref List<PlayerLayer> layers,
            ref AnimationComponent component)
        {
            layers.Clear();

            var currentFrame = component.CurrentKeyFrame;

            foreach (var layer in component.Animation.Layers.GetOrderedActiveLayerData(currentFrame))
            {
                var newLayer = new PlayerLayer(ModConstants.ModName, layer.Layer.Name, delegate (PlayerDrawInfo drawInfo)
                {
                    Main.playerDrawData.Add(SetDrawLayer(drawInfo, TextureUtils.GetTextureFromTag(layer.Layer.Tag), layer.Layer.Tag.Size, layer.FrameData, Color.White));
                });
                layers.Add(newLayer);
            }
        }

        public bool HandleMessage(ChangeAnimationMessage message)
        {
            if (!_changeMessages.Any(x => x.All(y => y.EntityId.Equals(message.EntityId))))
            {
                var queue = new Queue<ChangeAnimationMessage>();
                queue.Enqueue(message);
                _changeMessages.Add(queue);
                return false;
            }

            var current = _changeMessages.Single(x => x.All(y => y.EntityId.Equals(message.EntityId)));
            current.Enqueue(message);
            return true;
        }

        public bool HandleMessage(CreatePlayerLayersMessage message)
        {
            if (!EntityManager.EntityExists(message.EntityId))
                return false;

            var entity = EntityManager.GetEntity(message.EntityId);

            if (!HasComponent(entity))
                return false;

            CreatePlayerLayersForAnimation(ref message.PlayerLayers, ref GetComponent(entity));
            return true;
        }

        public override void PreEntityUpdate()
        {
            foreach (var entity in ReadEntities())
            {
                ref var component = ref ComponentManager.GetEntityComponent<AnimationComponent>(entity);

                if (InputCheckingUtils.PlayerInputBufferPaused())
                    continue;

                component.Animator.Update();

                var messages = _changeMessages.Single(x => x.All(y => y.EntityId.Equals(entity.Id)));
                int total = messages.Count;
                for (int m = 0; m < total; m++)
                {
                    component.Animation = messages.Dequeue().Animation;
                }
                _changeMessages.Remove(messages);
            }
        }

        protected override void OnCreate()
        {
            _changeMessages = new List<Queue<ChangeAnimationMessage>>();
        }
    }
}