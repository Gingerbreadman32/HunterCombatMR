using System;

namespace HunterCombatMR.Models
{
    public abstract class PositiveInteger<T>
        : IEquatable<int>,
        IComparable<int>,
        IFormattable
        where T : PositiveInteger<T>, new()
    {
        #region Private Fields

        private int _value;

        #endregion Private Fields

        #region Public Constructors

        public PositiveInteger()
        {
            _value = 0;
        }

        public PositiveInteger(int val)
        {
            if (val <= 0)
                throw new ArgumentOutOfRangeException("Value must be above 0!");

            _value = val;
        }

        #endregion Public Constructors

        #region Public Methods

        public int CompareTo(int other)
        {
            return _value.CompareTo(other);
        }

        public bool Equals(int other)
        {
            return _value.Equals(other);
        }

        public int Get()
            => _value;

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _value.ToString(format, formatProvider);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion Public Methods

        #region Operator Overrides

        public static int operator -(PositiveInteger<T> a, int b)
            => a._value - b;

        public static int operator +(PositiveInteger<T> a, int b)
            => a._value + b;

        public static int operator /(PositiveInteger<T> a, int b)
            => a._value / b;

        public static int operator *(PositiveInteger<T> a, int b)
            => a._value * b;

        public static bool operator ==(PositiveInteger<T> a, int b)
            => a._value.Equals(b);

        public static bool operator !=(PositiveInteger<T> a, int b)
            => !a._value.Equals(b);

        public static bool operator <=(PositiveInteger<T> a, int b)
            => a._value <= b;

        public static bool operator >=(PositiveInteger<T> a, int b)
            => a._value >= b;

        public static implicit operator int(PositiveInteger<T> d) 
            => d._value;

        public static explicit operator PositiveInteger<T>(int b) 
            => new T() { _value = b };

        #endregion Operator Overrides
    }
}