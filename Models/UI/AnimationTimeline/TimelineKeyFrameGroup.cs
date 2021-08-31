using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;
using Terraria.UI;

namespace HunterCombatMR.Models.UI.AnimationTimeline
{
    internal class TimelineKeyFrameGroup
        : UIElement,
        IComparable<TimelineKeyFrameGroup>
    {
        private const string _emptyFrame = UITexturePaths.TimelineTextures + "emptyframe";
        private const string _frame = UITexturePaths.TimelineTextures + "frame";
        private const string _fSeperator = UITexturePaths.TimelineTextures + "frameseperator";
        private const string _keyFrame = UITexturePaths.TimelineTextures + "keyframe";
        private const string _seperator = UITexturePaths.TimelineTextures + "seperator";
        private int _frames;
        private Texture2D _seperatorTexture;
        private Texture2D _texture;
        private FrameType _type;

        public TimelineKeyFrameGroup(Timeline parent,
            FrameType frameType,
            int keyFrameNumber,
            int scale,
            int frameAmount = 1)
        {
            ParentTimeline = parent;
            Keyframe = keyFrameNumber;
            _frames = frameAmount;
            Scale = scale;
            SetFrameType(frameType);
            SetTexture();
        }

        public bool IsActive { get; set; }

        public int Keyframe { get; }

        public Timeline ParentTimeline { get; }

        public int Scale { get; set; }

        public void ActivateKeyFrame()
        {
            if (!IsActive)
            {
                IsActive = true;

                SetTexture();
            }
        }

        public int CompareTo(TimelineKeyFrameGroup other)
        {
            if (other != null)
                return Keyframe.CompareTo(other.Keyframe);
            else
                throw new ArgumentNullException("Compared keyframe is null!");
        }

        public void DeactivateKeyFrame()
        {
            if (IsActive)
            {
                IsActive = false;

                SetTexture();
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            OnClick += (evt, list) =>
            {
                var animator = ParentTimeline.Animator;
                animator.SetToKeyframe(Keyframe);
                ParentTimeline.ResetFrames();
                HunterCombatMR.Instance.EditorInstance.AnimationEdited = true;
            };
        }

        public override void Recalculate()
        {
            base.Recalculate();
            Width.Set(CalculateWidth(), 0f);
            Height.Set(_texture.Height * Scale, 0f);
        }

        public void SetFrameType(FrameType newType)
        {
            _type = newType;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ParentTimeline.Animator.GetCurrentKeyframe().Equals(Keyframe))
                ActivateKeyFrame();
            else
                DeactivateKeyFrame();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            Vector2 position = GetDimensions().Position();

            spriteBatch.Draw(_seperatorTexture,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);

            position.X += 2f * Scale;

            spriteBatch.Draw(_texture,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);

            position.X += _texture.Width * Scale;

            if (_frames > 1 && ParentTimeline.ShowAllFrames)
            {
                for (int f = 1; f < _frames; f++)
                {
                    spriteBatch.Draw(ModContent.GetTexture(ActiveTexture(_fSeperator)),
                        position,
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Scale,
                        SpriteEffects.None,
                        0f);

                    position.X += 2f * Scale;

                    spriteBatch.Draw(ModContent.GetTexture(ActiveTexture(_frame)),
                        position,
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Scale,
                        SpriteEffects.None,
                        0f);

                    position.X += _texture.Width * Scale;
                }
            }

            spriteBatch.Draw(_seperatorTexture,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
        }

        private string ActiveTexture(string texture)
                    => IsActive ? texture + "active" : texture;

        private int CalculateWidth()
        {
            int totalFrameLength = !ParentTimeline.ShowAllFrames ? 1 : _frames;
            return (_texture.Width * Scale + _seperatorTexture.Width * Scale) * totalFrameLength + _seperatorTexture.Width * Scale;
        }

        private void SetTexture()
        {
            switch (_type)
            {
                case FrameType.Empty:
                    _texture = ModContent.GetTexture(ActiveTexture(_emptyFrame));
                    break;

                case FrameType.Keyframe:
                    _texture = ModContent.GetTexture(ActiveTexture(_keyFrame));
                    break;
            }

            _seperatorTexture = ModContent.GetTexture(ActiveTexture(_seperator));
        }
    }
}