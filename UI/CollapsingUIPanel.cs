using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace HunterCombatMR.UI
{
    public class CollapsingUIPanel
        : UIPanel
    {
        #region Public Properties

        public bool IsCollapsed { get; private set; }

        #endregion Public Properties

        public CollapsingUIPanel(bool startCollapsed)
        {
            IsCollapsed = startCollapsed;
        }

        #region Public Methods

        public void Reveal()
        {
            IsCollapsed = false;
        }

        public void Collapse()
        {
            IsCollapsed = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsCollapsed)
                base.Update(gameTime);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (!IsCollapsed)
                base.DrawChildren(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!IsCollapsed)
                base.DrawSelf(spriteBatch);
        }

        #endregion Protected Methods
    }
}