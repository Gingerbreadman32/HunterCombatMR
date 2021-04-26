using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace HunterCombatMR.UI.Elements
{
    public class CollapsingUIPanel
        : UIPanel
    {
        public bool IsCollapsed { get; private set; }

        public CollapsingUIPanel(bool startCollapsed)
        {
            IsCollapsed = startCollapsed;
        }

        public void Collapse()
        {
            IsCollapsed = true;
        }

        public void Reveal()
        {
            IsCollapsed = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsCollapsed)
                base.Update(gameTime);
        }

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
    }
}