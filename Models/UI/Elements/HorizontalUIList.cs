using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace HunterCombatMR.UI.Elements
{
    internal class HorizontalUIList<T>
        : UIList
        where T : UIElement
    {
        protected float _innerListWidth;

        /// <summary>
        /// Sorts everything in the list if it's of one derived type.
        /// </summary>
        /// <typeparam name="T">Derived type</typeparam>
        /// <remarks>
        /// Do not use if the list is multi-type!
        /// </remarks>
        public void AllTypeSort()
        {
            List<T> sorted = _items.Select(x => (T)x).ToList();
            sorted.Sort();
            _items = new List<UIElement>(sorted);
        }

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
            AllTypeSort();
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Left.Set(num, 0f);
                _items[i].Recalculate();
                num += _items[i].GetOuterDimensions().Width + ListPadding;
            }
            _innerListWidth = num;
        }
    }
}