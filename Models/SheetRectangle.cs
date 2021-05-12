using Microsoft.Xna.Framework;
using System;

namespace HunterCombatMR.Models
{
    public struct SheetRectangle
        : IEquatable<Rectangle>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int Frame;

        public SheetRectangle(Rectangle source,
            int frame = 0)
        {
            X = source.X;
            Y = source.Y * (frame + 1);
            Width = source.Width;
            Height = source.Height;
            Frame = frame;
        }

        public bool Equals(Rectangle other)
        {
            return other.X == X && other.Y == Y && other.Width == Width && other.Height == Height;
        }
    }
}