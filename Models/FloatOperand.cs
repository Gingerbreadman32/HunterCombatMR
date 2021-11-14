using HunterCombatMR.Interfaces;
using System;

namespace HunterCombatMR.Models
{
    public struct FloatOperand
        : IScriptElement,
        IEquatable<float>
    {
        public FloatOperand(float value)
        {
            NumericValue = value;
        }

        public float NumericValue { get; }
        
        public string Value { get => NumericValue.ToString(); }

        public static implicit operator FloatOperand(float b)
                    => new FloatOperand(b);

        public bool Equals(float other)
            => NumericValue.Equals(other);

        public string Solve(int entityId = -1)
                    => Value;

        public override string ToString()
            => Value;
    }
}