using Microsoft.Xna.Framework;

namespace HunterCombatMR.Extensions
{
    public static class RectangleExtensions
    {
        public static Rectangle SetSheetPositionFromFrame(this Rectangle rectangle,
            int frame)
        {
            rectangle.Y += (rectangle.Height * (frame)) + 2*frame;
            return rectangle;
        }
    }
}