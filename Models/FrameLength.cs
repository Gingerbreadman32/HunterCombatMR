using System;

namespace HunterCombatMR.Models
{
    public struct FrameLength
        : IEquatable<int>,
        IComparable<int>,
        IFormattable
    {
        private int _value;

        public FrameLength(int val)
        {
            if (val < Minimum)
                throw new ArgumentOutOfRangeException($"Length must be above {Minimum}!");
            _value = val;
        }

        public static int Minimum { get => 1; }
        public static FrameLength One { get => new FrameLength(1); }

        public int Value { get => _value; }

        public static explicit operator FrameLength(FrameIndex d)
            => new FrameLength(d.Value);

        public static implicit operator FrameLength(int b)
            => new FrameLength(b);

        public static implicit operator int(FrameLength d)
                                    => d._value;

        public static FrameLength operator -(FrameLength a, int b)
            => a._value - b;

        public static bool operator !=(FrameLength a, int b)
            => !a._value.Equals(b);

        public static int operator *(FrameLength a, int b)
            => a._value * b;

        public static int operator /(FrameLength a, int b)
            => a._value / b;

        public static FrameLength operator +(FrameLength a, int b)
            => a._value + b;

        public static bool operator <=(FrameLength a, int b)
            => a._value <= b;

        public static bool operator ==(FrameLength a, int b)
            => a._value.Equals(b);

        public static bool operator >=(FrameLength a, int b)
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