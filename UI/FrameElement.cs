using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    internal class FrameElement
        : UIElement
    {
        #region Private Fields

        private const string _emptyFrame = UITexturePaths.TimelineTextures + "emptyframe";
        private const string _frame = UITexturePaths.TimelineTextures + "frame";
        private const string _keyFrame = UITexturePaths.TimelineTextures + "keyframe";
        private const string _seperator = UITexturePaths.TimelineTextures + "seperator";
        private bool _active;
        private Texture2D _seperatorTexture;
        private Texture2D _texture;
        private FrameType _type;
        private int _frameNumber;

        #endregion Private Fields

        #region Public Constructors

        public FrameElement(FrameType frameType,
            int frameNumber)
        {
            _frameNumber = frameNumber;
            SetFrameType(frameType);
            SetTexture();
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);
        }

        #endregion Public Constructors

        #region Public Methods
        public override void OnInitialize()
        {
            //OnClick += (evt, list) => { ToggleActive(); };
        }

        public void SetActive(bool state)
        {
            _active = state;
        }

        public void SetFrameType(FrameType newType)
        {
            _type = newType;
        }

        public void ToggleActive()
        {
            _active ^= true;
        }

        #endregion Public Methods

        #region Private Methods

        private string ActiveTexture(string texture)
            => (_active) ? texture + "active" : texture;

        private void SetTexture()
        {
            switch (_type)
            {
                case FrameType.Empty:
                    _texture = ModContent.GetTexture(ActiveTexture(_emptyFrame));
                    break;

                case FrameType.Frame:
                    _texture = ModContent.GetTexture(ActiveTexture(_frame));
                    break;

                case FrameType.Keyframe:
                    _texture = ModContent.GetTexture(ActiveTexture(_keyFrame));
                    break;
            }

            _seperatorTexture = ModContent.GetTexture(ActiveTexture(_seperator));
        }

        #endregion Private Methods

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,
                GetDimensions().Position(),
                Color.White);
        }
    }
}