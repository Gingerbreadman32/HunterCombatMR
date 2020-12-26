using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace HunterCombatMR.UI
{
    internal class HorizontalUIList
        : UIList
    {
        #region Protected Fields

        protected float _innerListWidth;

        #endregion Protected Fields

        #region Public Methods

        public float GetTotalWidth()
        {
            return _innerListWidth;
        }

        public override void RecalculateChildren()
        {
            foreach (UIElement element in Elements)
            {
                element.Recalculate();
            }
            float num = 0f;
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Left.Set(num, 0f);
                _items[i].Recalculate();
                num += _items[i].GetOuterDimensions().Width + ListPadding;
            }
            _innerListWidth = num;
        }

        #endregion Public Methods
    }
}