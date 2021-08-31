using Microsoft.Xna.Framework;
using Terraria.UI;

namespace HunterCombatMR.Interfaces.UI
{
    internal interface IUIAttachable
    {
        /// <summary>
        /// The element this will be attached to
        /// </summary>
        UIElement AttachedElement { get; }

        /// <summary>
        /// The distance from the element this will be, measured between the distance from the centers of the objects.
        /// </summary>
        Vector2 AttachDistance { get; }

        /// <summary>
        /// Detatch the object from the element
        /// </summary>
        void Dettatch();

        /// <summary>
        /// Attach the object to a given element
        /// </summary>
        void Attatch(UIElement element,
            Vector2 distance);
    }
}