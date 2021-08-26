using HunterCombatMR.Constants;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.System;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.AnimationSystem;
using HunterCombatMR.Models.Animation;
using HunterCombatMR.Models.Animation.Entity;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Utilities;
using Microsoft.Xna.Framework;
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
            string layerName,
            EntityAnimationLayerData layerData,
            Color color)
        {
            var texture = TextureUtils.GetTextureFromTag(new TextureTag(layerName, Point.Zero));
            var frameRectangle = new Rectangle(0, 0, texture.Width, layerData.SheetIndex); // Change to loading the actual height based on a real texture tag set elsewhere.
            var drawPlayer = drawInfo.drawPlayer;

            var positionVector = new Vector2(drawInfo.position.X + drawPlayer.width / 2 - Main.screenPosition.X,
                        drawInfo.position.Y - Main.screenPosition.Y);

            frameRectangle = frameRectangle.SetSheetPositionFromFrame(layerData.SheetFrame);
            DrawData value = new DrawData(texture, layerData.Position.ToVector2(), frameRectangle, color);

            value = value.SetSpriteOrientation(drawPlayer, layerData.Orientation, frameRectangle);
            value.position += positionVector - frameRectangle.Size() / 2;

            return value;
        }

        public void CreatePlayerLayersForAnimation(ref List<PlayerLayer> layers,
            ref AnimationComponent component)
        {
            layers.Clear();

            var currentFrame = component.CurrentKeyFrame;

            foreach (var layer in component.Animation.GetOrderedActiveLayerData(currentFrame))
            {
                var newLayer = new PlayerLayer(ModConstants.ModName, layer.Key, delegate (PlayerDrawInfo drawInfo)
                {
                    Main.playerDrawData.Add(SetDrawLayer(drawInfo,
                        layer.Key,
                        layer.Value,
                        Color.White));
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
            if (InputCheckingUtils.PlayerInputBufferPaused())
                return;

            foreach (var entity in ReadEntities())
            {
                ref var component = ref entity.GetComponent<AnimationComponent>();

                component.Animator.Update();

                ChangeAnimation(entity, ref component);
            }

            _changeMessages.Clear();
        }

        protected override void OnCreate()
        {
            _changeMessages = new List<Queue<ChangeAnimationMessage>>();
        }

        private void ChangeAnimation(IModEntity entity, ref AnimationComponent component)
        {
            var messages = _changeMessages.SingleOrDefault(x => x.All(y => y.EntityId.Equals(entity.Id)));

            if (messages == null)
                return;

            int total = messages.Count;
            for (int m = 0; m < total; m++)
            {
                component.Animation = messages.Dequeue().Animation;
            }
        }
    }
}