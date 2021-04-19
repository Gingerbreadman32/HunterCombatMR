using HunterCombatMR.Extensions;
using System;

namespace HunterCombatMR.AnimationEngine.Models
{
    public struct FrameIndex
        : IEquatable<int>,
        IComparable<int>,
        IFormattable
    {
        private int _value;

        public FrameIndex(int val)
        {
            if (val < Minimum)
                throw new ArgumentOutOfRangeException($"Length must be above {Minimum}!");
            _value = val;
        }

        public static int Minimum { get => 0; }

        public static FrameIndex Zero { get => new FrameIndex(0); }

        public int Value { get => _value; }

        // @@warn Move this somewhere, way too messy.
        public static explicit operator FrameIndex(int b)
                    => new FrameIndex(b);

        public static explicit operator FrameIndex(FrameLength b)
                    => new FrameIndex(b);

        public static implicit operator FrameLength(FrameIndex d)
            => new FrameLength(d);

        public static implicit operator int(FrameIndex d)
                            => d._value;

        public static FrameIndex operator -(FrameIndex a, int b)
            => (a._value - b).ToFIndex();

        public static int operator -(int a, FrameIndex b)
            => a - b._value;

        public static FrameIndex operator -(FrameIndex a, FrameIndex b)
            => (a._value - b.Value).ToFIndex();

        public static bool operator !=(FrameIndex a, int b)
            => !a._value.Equals(b);

        public static int operator *(FrameIndex a, int b)
            => a._value * b;

        public static int operator /(FrameIndex a, int b)
            => a._value / b;

        public static int operator +(int a, FrameIndex b)
            => a + b._value;

        public static FrameIndex operator +(FrameIndex a, int b)
            => (a._value + b).ToFIndex();

        public static FrameIndex operator +(FrameIndex a, FrameLength b)
            => (a._value + b.Value).ToFIndex();

        public static bool operator <=(FrameIndex a, int b)
            => a._value <= b;

        public static bool operator ==(FrameIndex a, int b)
            => a._value.Equals(b);

        public static bool operator >=(FrameIndex a, int b)
            => a._value >= b;

        public int CompareTo(int other)
            => _value.CompareTo(other);

        public bool Equals(int other)
            => _value.Equals(other);

        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => _value.GetHashCode();

        public string ToString(string format, IFormatProvider formatProvider)
            => _value.ToString(format, formatProvider);

        public override string ToString()
            => _value.ToString();
    }
}